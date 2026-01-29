using System;
using System.Collections.Generic;

namespace ARSignTranslator.Core
{
    public class GestureBuffer
    {
        private readonly int _capacity;
        private readonly List<LandmarkFrame> _frames = new();

        public GestureBuffer(int capacity)
        {
            if (capacity <= 0) throw new ArgumentException("Capacity must be > 0");
            _capacity = capacity;
        }

        public int Count => _frames.Count;

        public void Clear()
        {
            _frames.Clear();
        }

        public void AddFrame(LandmarkFrame frame)
        {
            if (_frames.Count == _capacity)
                _frames.RemoveAt(0);

            _frames.Add(frame);
        }

        public bool IsFull()
        {
            return _frames.Count == _capacity;
        }

        public float[] ToFeatureVector()
        {
            if (_frames.Count == 0)
                throw new InvalidOperationException("Buffer is empty.");

            int frameSize = _frames[0].Values.Length;
            var features = new float[_frames.Count * frameSize];

            int idx = 0;
            foreach (var f in _frames)
            {
                if (f.Values.Length != frameSize)
                    throw new InvalidOperationException("Frame sizes are inconsistent.");

                Array.Copy(f.Values, 0, features, idx, frameSize);
                idx += frameSize;
            }

            return features;
        }
    }
}
