using System.Diagnostics;

namespace ARSignTranslator.Utils
{
    public class TimeUtils
    {
        private readonly Stopwatch _sw = new();

        public void Start()
        {
            _sw.Restart();
        }

        public long ElapsedMs()
        {
            return _sw.ElapsedMilliseconds;
        }

        public void Reset()
        {
            _sw.Reset();
        }
    }
}
