using System.Collections.Generic;

namespace SabTool.Data.Cinematics;

using SabTool.Utils;

public class DialogText
{
    public Crc Id { get; set; }
    public string VoiceOver { get; set; }
    public Crc VoiceOverCrc => new(Hash.StringToHash(VoiceOver));
    public string Text { get; set; }
}

public class Dialog
{
    public List<DialogText> Texts { get; set; }
    public Dictionary<Crc, List<DialogText>> SubTexts { get; set; }
}
