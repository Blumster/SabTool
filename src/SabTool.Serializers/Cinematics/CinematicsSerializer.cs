using System.Numerics;
using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Cinematics;
using SabTool.Data.Cinematics.CinematicElements;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Cinematics;
public static class CinematicsSerializer
{
    private const int Version = 12;

    private record CinematicEntry(Crc Name, int Offset, bool Unk);

    public static List<Cinematic> DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        int version = reader.ReadInt32();
        if (version != Version)
            throw new Exception($"Invalid version {version} for Cinematics!");

        int cinematicCount = reader.ReadInt32();

        List<CinematicEntry> entries = new(cinematicCount);
        List<Cinematic> cinematics = new(cinematicCount);

        for (int i = 0; i < cinematicCount; ++i)
        {
            entries.Add(new CinematicEntry(new(reader.ReadUInt32()), reader.ReadInt32(), reader.ReadBoolean()));
        }

        foreach (CinematicEntry entry in entries)
        {
            stream.Position = entry.Offset;

            cinematics.Add(DeserializeCinematic(reader, entry.Name));
        }

        return cinematics;
    }

    private static Cinematic DeserializeCinematic(BinaryReader reader, Crc name)
    {
        short version = reader.ReadInt16();
        if (version != Version)
            throw new Exception($"Invalid version {version} for Cinematic!");

        Cinematic cinematic = new(name);

        int isStreaming = -1;

        while (true)
        {
            uint elementTag = reader.ReadUInt32();

            if (elementTag == 0x43454E44) // CEND TODO
            {
                break;
            }

            switch (elementTag)
            {
                case 0xDA5C4B83: // Hash("<UNKNOWN>")
                    cinematic.Field1F6 &= 0xFD;
                    break;

                case 0xCBA918EA: // Hash("<UNKNOWN>")
                    cinematic.Field60 = reader.ReadSingle();
                    break;

                case 0xD98B7848: // Hash("MotionBlur")
                    CinemaMotionBlur motionBlur = new()
                    {
                        StartTime = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        Strength = reader.ReadSingle()
                    };
                    cinematic.Elements.Add(motionBlur);

                    if (cinematic.Duration < motionBlur.EndTime)
                        cinematic.Duration = motionBlur.EndTime;

                    break;

                case 0xD0B107BB: // Hash("Sound3D")
                    CinemaSound3D sound3D = new()
                    {
                        StartTime = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        SoundName = reader.ReadStringWithMaxLength(reader.ReadInt16()),
                        UnkBool = reader.ReadInt16() != 0,
                        UnkCrc = new(reader.ReadUInt32())
                    };

                    // Store hash
                    _ = Hash.FNV32string(sound3D.SoundName);

                    cinematic.Elements.Add(sound3D);

                    if (cinematic.Duration < sound3D.EndTime)
                        cinematic.Duration = sound3D.EndTime;

                    break;

                case 0xE6CE9B69: // Hash("Subtitle")
                    CinemaSubtitle subtitle = new()
                    {
                        StartTime = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        UnkInt1 = reader.ReadInt32(),
                        UnkInt2 = reader.ReadInt32()
                    };

                    if (cinematic.Duration < subtitle.EndTime)
                        cinematic.Duration = subtitle.EndTime;

                    cinematic.Elements.Add(subtitle);

                    uint tag = reader.ReadUInt32();
                    if (tag != 0x53545246) // STRF
                    {
                        reader.BaseStream.Position -= 4;
                        break;
                    }

                    subtitle.UnkCrc = new(reader.ReadUInt32());
                    break;

                case 0xF49470BA: // Hash("Rumble")
                    CinemaRumble rumble = new()
                    {
                        StartTime = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        Strength = reader.ReadSingle(),
                        Unk = reader.ReadSingle()
                    };

                    cinematic.Elements.Add(rumble);

                    if (cinematic.Duration < rumble.EndTime)
                        cinematic.Duration = rumble.EndTime;

                    break;

                case 0xEBB11A67: // Hash("SetDamageState")
                    CinemaDamageState damageState = new()
                    {
                        EndTime = reader.ReadSingle(),
                        UnkCrc1 = new(reader.ReadUInt32()),
                        UnkCrc2 = new(reader.ReadUInt32())
                    };

                    cinematic.Elements.Add(damageState);

                    if (cinematic.Duration < damageState.EndTime)
                        cinematic.Duration = damageState.EndTime;

                    break;

                case 0xB4D9B99B: // Hash("AttachObject")
                    CinemaAttachObject attachObject = new()
                    {
                        Unk = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        UnkInt1 = reader.ReadInt32(),
                        UnkInt2 = reader.ReadInt32(),
                        UnkInt3 = reader.ReadInt32(),
                        UnkInt4 = reader.ReadInt32(),
                    };

                    cinematic.Elements.Add(attachObject);

                    if (cinematic.Duration < attachObject.EndTime)
                        cinematic.Duration = attachObject.EndTime;

                    break;

                case 0x9CC86FE3: // Hash("FlashMovie")
                    CinemaFlashMovie flashMovie = new()
                    {
                        EndTime = reader.ReadSingle(),
                        Name = reader.ReadStringWithMaxLength(reader.ReadInt16())
                    };

                    // Store hash
                    _ = Hash.FNV32string(flashMovie.Name);

                    cinematic.Elements.Add(flashMovie);

                    if (cinematic.Duration < flashMovie.EndTime)
                        cinematic.Duration = flashMovie.EndTime + 0.01f;

                    break;

                case 0xA9DED8D5: // Hash("BinkMovie")
                    CinemaBinkMovie binkMovie = new()
                    {
                        EndTime = reader.ReadSingle(),
                        Name = reader.ReadStringWithMaxLength(reader.ReadInt16())
                    };

                    cinematic.Field1F5 |= 0x20;

                    // Store hash
                    _ = Hash.FNV32string(binkMovie.Name);

                    cinematic.Elements.Add(binkMovie);

                    if (cinematic.Duration < binkMovie.EndTime)
                        cinematic.Duration = binkMovie.EndTime + 0.01f;

                    uint tag2 = reader.ReadUInt32();
                    if (tag2 != 0x50415553) // PAUS
                    {
                        reader.BaseStream.Position -= 4;
                        break;
                    }

                    binkMovie.IsPaused = true;
                    break;

                case 0x9F8BCA10: // Hash("SoundBank")
                    CinemaSoundBank soundBank = new()
                    {
                        Unk = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        Name = reader.ReadStringWithMaxLength(reader.ReadInt16())
                    };

                    // Store hash
                    _ = Hash.FNV32string(soundBank.Name);

                    if (soundBank.Unk <= 0.0f)
                    {
                        soundBank.Flags |= 0x3;

                        cinematic.Field1F4 |= 0x1;
                    }

                    cinematic.Elements.Add(soundBank);

                    if (cinematic.Duration < soundBank.EndTime)
                        cinematic.Duration = soundBank.EndTime;

                    if (reader.ReadUInt32() != 0x43454E44)
                        throw new Exception("Invalid end tag found for CinemaSoundBank!");

                    break;

                case 0x9990EDD9: // Hash("UseAttractionPoint")
                    CinemaAttractionPt attractionPt = new()
                    {
                        EndTime = reader.ReadSingle(),
                        UnkCrc1 = new(reader.ReadUInt32()),
                        UnkCrc2 = new(reader.ReadUInt32()),
                    };

                    cinematic.Elements.Add(attractionPt);

                    if (cinematic.Duration < attractionPt.EndTime)
                        cinematic.Duration = attractionPt.EndTime;

                    break;

                case 0x86BF6C5B: // Hash("FX")
                    CinemaFX fx = new()
                    {
                        EndTime = reader.ReadSingle(),
                        UnkCrc1 = new(reader.ReadUInt32()),
                        UnkCrc2 = new(reader.ReadUInt32()),
                        UnkCrc3 = new(reader.ReadUInt32()),
                    };

                    cinematic.Elements.Add(fx);

                    if (cinematic.Duration < fx.EndTime)
                        cinematic.Duration = fx.EndTime;

                    break;

                case 0x7BBE9A2B: // Hash("SetMusicState")
                    CinemaMusicState musicState = new()
                    {
                        EndTime = reader.ReadSingle(),
                        Unk = reader.ReadInt16() != 0
                    };

                    cinematic.Elements.Add(musicState);

                    if (cinematic.Duration < musicState.EndTime)
                        cinematic.Duration = musicState.EndTime;

                    break;

                case 0x84B625DC: // Hash("LoadBlock")
                    CinemaLoadBlock loadBLock = new()
                    {
                        EndTime = reader.ReadSingle(),
                        Name = reader.ReadStringWithMaxLength(reader.ReadInt16())
                    };

                    // Store hash
                    _ = Hash.FNV32string(loadBLock.Name);

                    cinematic.Elements.Add(loadBLock);

                    if (loadBLock.EndTime > 0.0f && cinematic.Duration < loadBLock.EndTime)
                        cinematic.Duration = loadBLock.EndTime;

                    break;

                case 0x8D1B88C3: // Hash("PlayAnimation")
                    CinemaAnimation animation = new()
                    {
                        UnkFloat1 = reader.ReadSingle(),
                        UnkFloat2 = reader.ReadSingle(),
                        UnkFloat3 = reader.ReadSingle(),
                        UnkCrc1 = new(reader.ReadUInt32()),
                        Name = reader.ReadStringWithMaxLength(reader.ReadInt16()),
                        UnkCrc2 = new(reader.ReadUInt32()),
                    };

                    // Store hash
                    _ = Hash.FNV32string(animation.Name);

                    cinematic.Elements.Add(animation);

                    if (cinematic.Duration < animation.UnkFloat1 + animation.UnkFloat2)
                        cinematic.Duration = animation.UnkFloat1 + animation.UnkFloat2;

                    if (cinematic.Duration < animation.UnkFloat1)
                        cinematic.Duration = animation.UnkFloat1;

                    if (reader.ReadUInt32() != 0x43454E44)
                        throw new Exception("Invalid end tag found for CinemaAnimation!");

                    if (cinematic.Field54 + 5.0f > animation.UnkFloat1)
                        animation.Flags |= 0x2;

                    break;

                case 0x7447097D: // Hash("<UNKNOWN>")
                    cinematic.Field1F7 |= 0x20;
                    break;

                case 0x6FEEF89F: // Hash("") - CinemaLight
                    CinemaLight light = new()
                    {
                        EndTime = reader.ReadSingle(),
                        UnkBool1 = reader.ReadInt16() != 0,
                        UnkInt = reader.ReadInt32(),
                        UnkBool2 = reader.ReadInt16() != 0,
                    };

                    cinematic.Elements.Add(light);

                    if (cinematic.Duration < light.EndTime)
                        cinematic.Duration = light.EndTime;

                    break;

                case 0x7213E378: // Hash("FaceFX")
                    CinemaFaceFX faceFx = new()
                    {
                        UnkFloat1 = reader.ReadSingle(),
                        UnkFloat2 = reader.ReadSingle(),
                        UnkInt1 = reader.ReadInt32(),
                        UnkInt2 = reader.ReadInt32(),
                        UnusedFloat = reader.ReadSingle()
                    };

                    cinematic.Elements.Add(faceFx);

                    if (cinematic.Duration < faceFx.UnkFloat1)
                        cinematic.Duration = faceFx.UnkFloat1;

                    break;

                case 0x6F96C6EE: // Hash("<UNKNOWN>")
                    cinematic.Field1F5 &= 0xF7;
                    break;

                case 0x6DC9D9B2: // Hash("<UNKNOWN>")
                    cinematic.Field1F4 |= 0x80;
                    cinematic.Field1F5 &= 0xFE;
                    break;

                case 0x6D17D447: // Hash("<UNKNOWN>")
                    cinematic.UnkSubs ??= new Cinematic.UnkSub[25];

                    Crc crc = new(reader.ReadUInt32());

                    if (cinematic.UnkSubCount < 25)
                    {
                        cinematic.UnkSubs[cinematic.UnkSubCount].A = crc;

                        ++cinematic.UnkSubCount;
                    }

                    if (cinematic.UnkSubCount > 0)
                        cinematic.Field1F4 |= 0x2;

                    if (reader.ReadUInt32() != 0x43454E44)
                        throw new Exception("Invalid end tag found for CinemaAnimation!");

                    break;

                case 0x6A8F7467: // Hash("CameraSpline")
                    CinemaSplineCamera splineCamera = new()
                    {
                        UnkFloat = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        UnkCrc1 = new(reader.ReadUInt32()),
                        UnkCrc2 = new(reader.ReadUInt32()),
                        UnkBool = reader.ReadInt16() != 0
                    };

                    cinematic.Elements.Add(splineCamera);
                    cinematic.Field1F4 |= 0x40;

                    if (cinematic.Duration < splineCamera.EndTime)
                        cinematic.Duration = splineCamera.EndTime;

                    break;

                case 0x427CC1F8: // Hash("Sound2D")
                    CinemaSound2D sound2D = new()
                    {
                        UnkFloat = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        Name = reader.ReadStringWithMaxLength(reader.ReadInt16()),
                        UnkBool = reader.ReadInt16() != 0
                    };

                    // Store hash
                    _ = Hash.FNV32string(sound2D.Name);

                    cinematic.Elements.Add(sound2D);

                    if (cinematic.Duration < sound2D.EndTime)
                        cinematic.Duration = sound2D.EndTime;

                    break;

                case 0x3F0B90FC: // Hash("Teleport")
                    CinemaTeleport teleport = new()
                    {
                        EndTime = reader.ReadSingle(),
                        UnkCrc1 = new(reader.ReadUInt32()),
                        UnkCrc2 = new(reader.ReadUInt32())
                    };

                    cinematic.Elements.Add(teleport);

                    if (cinematic.Duration < teleport.EndTime)
                        cinematic.Duration = teleport.EndTime;

                    uint tag3 = reader.ReadUInt32();
                    if (tag3 == 0x534B4950) // SKIP
                    {
                        teleport.Flags &= 0xFD;

                        tag3 = reader.ReadUInt32();
                    }

                    if (tag3 == 0x4E544F4C) // NTOL
                    {
                        teleport.Flags &= 0xFB;
                        break;
                    }

                    reader.BaseStream.Position -= 4;
                    break;

                case 0x412D1576: // Hash("CameraShake")
                    CinemaCameraShake cameraShake = new()
                    {
                        EndTime = reader.ReadSingle(),
                        UnkCrc = new(reader.ReadUInt32()),
                        Strength = reader.ReadSingle(),
                        UnkFloat1 = reader.ReadSingle(),
                        UnkFloat2 = reader.ReadSingle(),
                    };

                    cinematic.Elements.Add(cameraShake);

                    if (cinematic.Duration < cameraShake.EndTime)
                        cinematic.Duration = cameraShake.EndTime;

                    break;

                case 0x3C7A73EB: // Hash("MoveObject")
                    CinemaObject obj = new()
                    {
                        UnkFloat1 = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        UnkCrc1 = new(reader.ReadUInt32()),
                        UnkCrc2 = new(reader.ReadUInt32()),
                        UnkInt = reader.ReadInt32(),
                        UnkBool1 = reader.ReadInt16() != 0,
                        UnkBool2 = reader.ReadInt16() != 0,
                        UnkBool3 = reader.ReadInt16() != 0,
                    };

                    cinematic.Elements.Add(obj);

                    if (cinematic.Duration < obj.EndTime)
                        cinematic.Duration = obj.EndTime;

                    break;

                case 0x3049BAAB: // Hash("<UNKNOWN>")
                    cinematic.Field1AC &= 0xFE;
                    cinematic.Field1F7 |= 0x1E;

                    uint tag4 = reader.ReadUInt32();
                    if (tag4 is 0x54524E32 or 0x54524E33) // TRN2 || TRN3
                    {
                        if (reader.ReadInt32() != 0)
                        {
                            cinematic.Field1AC |= 0x2;
                        }
                        else
                        {
                            cinematic.Field1AC &= 0xFD;
                        }

                        Crc speedCrc = new(reader.ReadUInt32());
                        cinematic.Field19C = cinematic.Field198 = speedCrc.Value == Hash.StringToHash("Fast") ? 0.3f : 0.6f;

                        if (reader.ReadInt32() != 0)
                        {
                            cinematic.Field1AC |= 0x4;
                        }
                        else
                        {
                            cinematic.Field1AC &= 0xFB;
                        }

                        speedCrc = new Crc(reader.ReadUInt32());
                        cinematic.Field1A0 = cinematic.Field1A4 = speedCrc.Value == Hash.StringToHash("Fast") ? 0.3f : 0.6f;
                        cinematic.Field1A8 = reader.ReadSingle();
                        cinematic.Field194 = reader.ReadInt32();

                        if (tag4 == 0x54524E33) // TRN3
                        {
                            if (reader.ReadInt16() != 0)
                            {
                                cinematic.Field1F7 |= 0x2;
                            }
                            else
                            {
                                cinematic.Field1F7 &= 0xFD;
                            }

                            if (reader.ReadInt16() != 0)
                            {
                                cinematic.Field1F7 |= 0x4;
                            }
                            else
                            {
                                cinematic.Field1F7 &= 0xFB;
                            }
                        }
                    }
                    else if (tag4 == 0x54524E34) // TRN4
                    {
                        if (reader.ReadInt32() != 0)
                        {
                            cinematic.Field1AC |= 0x2;
                        }
                        else
                        {
                            cinematic.Field1AC &= 0xFD;
                        }

                        cinematic.Field198 = reader.ReadSingle();
                        if (cinematic.Field198 < 0.5f)
                            cinematic.Field198 = 0.5f;

                        cinematic.Field19C = reader.ReadSingle();
                        if (cinematic.Field19C < 0.5f)
                            cinematic.Field19C = 0.5f;

                        if (reader.ReadInt32() != 0)
                        {
                            cinematic.Field1AC |= 0x4;
                        }
                        else
                        {
                            cinematic.Field1AC &= 0xFB;
                        }

                        cinematic.Field1A0 = reader.ReadSingle();
                        if (cinematic.Field1A0 < 0.5f)
                            cinematic.Field1A0 = 0.5f;

                        cinematic.Field1A4 = reader.ReadSingle();
                        if (cinematic.Field1A4 < 0.5f)
                            cinematic.Field1A4 = 0.5f;

                        cinematic.Field1A8 = reader.ReadSingle();
                        if (cinematic.Field1A8 < 0.5f)
                            cinematic.Field1A8 = 0.5f;

                        cinematic.Field194 = reader.ReadInt32();

                        if (reader.ReadInt16() != 0)
                        {
                            cinematic.Field1F7 |= 0x2;
                        }
                        else
                        {
                            cinematic.Field1F7 &= 0xFD;
                        }

                        if (reader.ReadInt16() != 0)
                        {
                            cinematic.Field1F7 |= 0x4;
                        }
                        else
                        {
                            cinematic.Field1F7 &= 0xFB;
                        }
                    }
                    else if (tag4 == 0x54524E31) // TRN1
                    {
                        if (reader.ReadInt32() != 0)
                        {
                            cinematic.Field1AC |= 0x6;
                        }
                        else
                        {
                            cinematic.Field1AC &= 0xF9;
                        }

                        cinematic.Field194 = reader.ReadInt32();

                        Crc speedCrc = new(reader.ReadUInt32());
                        cinematic.Field1A0 = cinematic.Field1A4 = cinematic.Field19C = cinematic.Field198 = speedCrc.Value == Hash.StringToHash("Fast") ? 0.3f : 0.6f;

                        cinematic.Field1A8 = reader.ReadSingle();
                    }
                    break;

                case 0x3B743523: // Hash("Streaming")
                    CinemaStreaming streaming = new()
                    {
                        EndTime = reader.ReadSingle(),
                        Enable = reader.ReadInt16() == 1
                    };

                    cinematic.Elements.Add(streaming);

                    if (cinematic.Duration < streaming.EndTime)
                        cinematic.Duration = streaming.EndTime;

                    if (streaming.EndTime <= 0.0f)
                        isStreaming = streaming.Enable ? 1 : 0;

                    if (reader.ReadUInt32() != 0x43454E44)
                        throw new Exception("Invalid end tag found for CinemaAnimation!");

                    break;

                case 0x2B941A68: // Hash("ShadowQuality")
                    CinemaShadowQuality shadowQuality = new()
                    {
                        EndTime = reader.ReadSingle(),
                        Quality = new(reader.ReadUInt32())
                    };

                    cinematic.Elements.Add(shadowQuality);

                    if (cinematic.Duration < shadowQuality.EndTime)
                        cinematic.Duration = shadowQuality.EndTime;

                    break;

                case 0x16930AFE: // Hash("Explosion")
                    CinemaExplosion explosion = new()
                    {
                        EndTime = reader.ReadSingle(),
                        UnkCrc1 = new(reader.ReadUInt32()),
                        UnkCrc2 = new(reader.ReadUInt32())
                    };

                    cinematic.Elements.Add(explosion);

                    if (cinematic.Duration < explosion.EndTime)
                        cinematic.Duration = explosion.EndTime;

                    break;

                case 0x5586466: // Hash("BloodSplatter")
                    CinemaBloodSplatter bloodSplatter = new()
                    {
                        StartTime = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        UnkFloat2 = reader.ReadSingle(),
                        UnkFloat3 = reader.ReadSingle(),
                        UnkBool = reader.ReadInt16() != 0
                    };

                    cinematic.Elements.Add(bloodSplatter);

                    if (cinematic.Duration < bloodSplatter.EndTime)
                        cinematic.Duration = bloodSplatter.EndTime;

                    break;

                case 0x12902319: // Hash("ObjectMotionBlur")
                    CinemaObjectMotionBlur objectMotionBlur = new()
                    {
                        StartTime = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        UnkCrc = new(reader.ReadUInt32())
                    };

                    cinematic.Elements.Add(objectMotionBlur);

                    if (cinematic.Duration < objectMotionBlur.EndTime)
                        cinematic.Duration = objectMotionBlur.EndTime;

                    uint tag5 = reader.ReadUInt32();
                    if (tag5 == 0x41504C59) // APLY
                    {
                        objectMotionBlur.Flags |= 0x1;
                        break;
                    }

                    reader.BaseStream.Position -= 4;
                    break;

                case 0x1AFCF1CE: // Hash("<UNKNOWN>")
                    cinematic.Field1F5 |= 0x4;
                    break;

                case 0x1BD88DB7: // Hash("AnimateObject")
                    CinemaAnimateObject animateObject = new()
                    {
                        UnkFloat1 = reader.ReadSingle(),
                        UnkFloat2 = reader.ReadSingle(),
                        UnkFloat3 = reader.ReadSingle(),
                        UnkCrc1 = new(reader.ReadUInt32()),
                        Name = reader.ReadStringWithMaxLength(reader.ReadInt16()),
                        UnkCrc2 = new(reader.ReadUInt32()),
                    };

                    // Store hash
                    _ = Hash.FNV32string(animateObject.Name);

                    cinematic.Elements.Add(animateObject);

                    if (cinematic.Duration < animateObject.UnkFloat1 + animateObject.UnkFloat2)
                        cinematic.Duration = animateObject.UnkFloat1 + animateObject.UnkFloat2;

                    if (cinematic.Field54 + 5.0f > animateObject.UnkFloat1)
                        animateObject.Flags |= 0x2;

                    if (reader.ReadUInt32() != 0x43454E44)
                        throw new Exception("Invalid end tag found for CinemaAnimation!");

                    break;

                case 0x60D1BB07: // Hash("UnloadBlock")
                    CinemaUnloadBlock unloadBlock = new()
                    {
                        EndTime = reader.ReadSingle(),
                        UnkCrc = new(reader.ReadUInt32()),
                    };

                    cinematic.Elements.Add(unloadBlock);

                    if (cinematic.Duration < unloadBlock.EndTime)
                        cinematic.Duration = unloadBlock.EndTime;

                    break;

                case 0x57BA4F5A: // Hash("<UNKNOWN>")
                    cinematic.Field1E8 = reader.ReadInt32();
                    break;

                case 0x6090527C: // Hash("CameraXSI")
                    CinemaXSICamera cameraXSI = new()
                    {
                        UnkFloat1 = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                    };

                    if (cinematic.Duration < cameraXSI.EndTime)
                        cinematic.Duration = cameraXSI.EndTime;

                    cameraXSI.Subs = new CinemaXSICamera.Sub[reader.ReadInt32()];
                    for (int i = 0; i < cameraXSI.Subs.Length; ++i)
                    {
                        while (true)
                        {
                            uint tag6 = reader.ReadUInt32();
                            if (tag6 == 0x4B454E44) // KEND
                                break;

                            if (tag6 == 0x4B4D4154) // KMAT
                            {
                                cameraXSI.Subs[i].Matrix = new Matrix4x4(
                                    reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0.0f,
                                    reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0.0f,
                                    reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0.0f,
                                    reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1.0f);
                            }
                            else if (tag6 == 0x4B464F56) // KFOV
                            {
                                cameraXSI.Subs[i].FOV = reader.ReadSingle();
                            }
                            else if (tag6 == 0x4B434F43) // KCOC
                            {
                                cameraXSI.Subs[i].F84 = reader.ReadSingle();
                            }
                            else if (tag6 == 0x4B444634) // KDF4
                            {
                                cameraXSI.Subs[i].F68 = reader.ReadSingle();
                                cameraXSI.Subs[i].F72 = reader.ReadSingle();
                                cameraXSI.Subs[i].F76 = reader.ReadSingle();
                                cameraXSI.Subs[i].F80 = reader.ReadSingle();
                            }
                        }
                    }

                    cameraXSI.UnkInt = reader.ReadInt32();

                    cinematic.Elements.Add(cameraXSI);
                    cinematic.Field1F4 |= 0x40;
                    break;

                case 0x65C52B0F: // Hash("PlayConvLine")
                    CinemaConvLine convLine = new()
                    {
                        EndTime = reader.ReadSingle(),
                        UnkInt1 = reader.ReadInt32(),
                        UnkInt2 = reader.ReadInt32(),
                    };

                    cinematic.Elements.Add(convLine);

                    if (cinematic.Duration < convLine.EndTime)
                        cinematic.Duration = convLine.EndTime;

                    break;

                case 0x66364EDB: // Hash("LODDistance")
                    CinemaLODDistance LODDistance = new()
                    {
                        StartTime = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        UnkFloat1 = reader.ReadSingle(),
                        UnkFloat2 = reader.ReadSingle()
                    };

                    cinematic.Elements.Add(LODDistance);

                    if (cinematic.Duration < LODDistance.EndTime)
                        cinematic.Duration = LODDistance.EndTime;

                    break;

                case 0x55377182: // Hash("PlayConversation")
                    CinemaConversation conversation = new()
                    {
                        EndTime = reader.ReadSingle(),
                        UnkCrc = new(reader.ReadUInt32())
                    };

                    cinematic.Elements.Add(conversation);

                    if (cinematic.Duration < conversation.EndTime)
                        cinematic.Duration = conversation.EndTime;

                    break;

                case 0x4B39ED5C: // Hash("TimeScale")
                    CinemaTimeScale timeScale = new()
                    {
                        StartTime = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        Scale = reader.ReadSingle(),
                    };

                    cinematic.Elements.Add(timeScale);

                    if (cinematic.Duration < timeScale.StartTime + timeScale.EndTime)
                        cinematic.Duration = timeScale.StartTime + timeScale.EndTime;

                    break;

                case 0x4D7649EC: // Hash("LuaScript")
                    CinemaScript script = new()
                    {
                        EndTime = reader.ReadSingle(),
                        Script = reader.ReadStringWithMaxLength(reader.ReadInt16()),
                    };

                    // Store hash
                    _ = Hash.FNV32string(script.Script);

                    if (reader.ReadInt16() != 0)
                        script.Flags |= 0x2;
                    else
                        script.Flags &= 0xFD;

                    cinematic.Elements.Add(script);

                    if (cinematic.Duration < script.EndTime)
                        cinematic.Duration = script.EndTime;

                    uint tag7 = reader.ReadUInt32();
                    if (tag7 == 0x4C434D44) // LCMD
                    {
                        script.Flags |= 0x4;
                        break;
                    }

                    reader.BaseStream.Position -= 4;
                    break;

                case 0x4A09E56C: // Hash("Camera")
                    CinemaCamera camera = new()
                    {
                        StartTime = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                        UnkCrc1 = new(reader.ReadUInt32()),
                        UnkCrc2 = new(reader.ReadUInt32()),
                        UnkCrc3 = new(reader.ReadUInt32()),
                        UnkCrc4 = new(reader.ReadUInt32())
                    };

                    cinematic.Elements.Add(camera);
                    cinematic.Field1F4 |= 0x40;

                    if (cinematic.Duration < camera.EndTime)
                        cinematic.Duration = camera.EndTime;

                    if (reader.ReadUInt32() != 0x43454E44)
                        throw new Exception("Invalid end tag found for CinemaAnimation!");

                    break;

                case 0x486725BB: // Hash("Fade")
                    CinemaFade fade = new()
                    {
                        StartTime = reader.ReadSingle(),
                        EndTime = reader.ReadSingle(),
                    };

                    cinematic.Elements.Add(fade);

                    if (cinematic.Duration < fade.StartTime)
                        cinematic.Duration = fade.StartTime;

                    uint methodCrc = reader.ReadUInt32();

                    float opacity = 0.0f;
                    bool hasOpacity = false;

                    uint tag8 = reader.ReadUInt32();
                    if (tag8 == 0x4F504143) // OPAC
                    {
                        hasOpacity = true;

                        opacity = reader.ReadSingle();
                    }
                    else if (tag8 == 0x43454E44) // CEND
                    {
                        reader.BaseStream.Position -= 4;
                    }

                    switch (methodCrc)
                    {
                        case 0x17A2F7BA:
                            fade.UnkInt = hasOpacity ? (uint)(byte)(255.0f * opacity) << 24 : 0x00000000;
                            break;

                        case 0x9EBA1E81:
                            fade.UnkInt = hasOpacity ? 0x00FFFFFF | ((uint)(byte)(255.0f * opacity) << 24) : 0xFFFFFFFF;
                            break;

                        case 0xDEDDFB47:
                            if (hasOpacity)
                            {
                                fade.UnkInt = (uint)(byte)(255.0f * opacity) << 24; // TODO
                            }
                            else
                            {
                                fade.UnkInt = 0xFF000000;
                            }
                            break;

                        default:
                            fade.UnkInt = hasOpacity ? 0x00FFFFFFu | ((uint)(byte)(255.0f * opacity) << 24) : 0x00FFFFFF;
                            break;
                    }

                    break;
            }
        }

        if (isStreaming != -1)
        {
            if (isStreaming == 0)
                cinematic.Field1F6 &= 0xFE;
            else
                cinematic.Field1F6 |= 0x1;
        }
        else
        {
            if (cinematic.UnkSubCount == 0)
                cinematic.Field1F6 &= 0xFE;
            else
                cinematic.Field1F6 |= 0x1;
        }

        return cinematic;
    }

    public static void SerialzieRaw(List<Cinematic> cinematics, Stream stream)
    {

    }

    public static List<Cinematic>? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(List<Cinematic> cinematics, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(cinematics, Formatting.Indented, new CrcConverter()));
    }
}
