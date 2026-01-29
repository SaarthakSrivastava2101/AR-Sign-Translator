using System;

namespace ARSignTranslator.Output
{
    public class ConsoleTTS
    {
        public void Speak(string text)
        {
            Console.WriteLine($"[TTS] {text}");
        }
    }
}
