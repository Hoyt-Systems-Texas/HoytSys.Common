using System;
using System.Linq;
using System.Text;
using A19.Messaging;

namespace Mrh.Messaging.Json
{
    public class JsonBodyReconstructor : IBodyReconstructor<string>
    {
        private int received;
        private string[] fragments;

        public JsonBodyReconstructor(int total)
        {
            this.fragments = new string[total];
        }

        public void Append(int position, string body)
        {
            if (position < this.fragments.Length)
            {
                if (this.fragments[position] == null)
                {
                    this.fragments[position] = body;
                    received++;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(position), position,
                    $"Expected a number less than {this.fragments.Length}");
            }
        }

        public void MissingFragments(Action<int> handler)
        {
            for (var i = 0; i < this.fragments.Length; i++)
            {
                if (this.fragments[i] == null)
                {
                    handler(i);
                }
            }
        }

        public bool Completed()
        {
            return received == fragments.Length;
        }

        public string Body
        {
            get
            {
                if (this.Completed())
                {
                    var builder = new StringBuilder(TotalLengthFast());
                    for (var i = 0; i < this.fragments.Length; i++)
                    {
                        builder.Append(this.fragments[i]);
                    }

                    return builder.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        private int TotalLengthFast()
        {
            var sum = 0;
            for (var i = 0; i < this.fragments.Length; i++)
            {
                sum += this.fragments[i].Length;
            }

            return sum;
        }
    }
}