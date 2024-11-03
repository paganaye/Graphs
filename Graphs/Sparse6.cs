using System.Collections.Generic;
using System.Text;

namespace Graphs;

using System;

public static class Sparse6
{
    public static string? Serialize(Graph graph)
    {
        int n = graph.NodeCount;

        // Initialize StringBuilder with the ':' prefix
        StringBuilder sb = new StringBuilder();
        sb.Append(':');

        // Encode the number of nodes
        AppendInteger(sb, n);

        // Encode the edges
        int last = -1;
        int k = 0; // Number of consecutive '1' bits
        int nb = 0; // Number of bits accumulated
        int accum = 0; // Accumulator for bits

        for (int i = 0; i < n; i++)
        {
            foreach (int j in graph.ForEachNeighbor(i))
            {
                if (j > i)
                {
                    int d = j - last - 1;
                    last = j;

                    // Encode the number of leading '1's
                    while (d >= (1 << k))
                    {
                        accum = (accum << 1) | 1;
                        nb++;
                        if (nb == 6)
                        {
                            sb.Append((char)(accum + 63));
                            accum = 0;
                            nb = 0;
                        }

                        d -= 1 << k;
                        k++;
                    }

                    // Append a '0' bit
                    accum = (accum << 1) | 0;
                    nb++;
                    if (nb == 6)
                    {
                        sb.Append((char)(accum + 63));
                        accum = 0;
                        nb = 0;
                    }

                    // Append 'k' bits representing 'd'
                    for (int s = k - 1; s >= 0; s--)
                    {
                        int bit = (d >> s) & 1;
                        accum = (accum << 1) | bit;
                        nb++;
                        if (nb == 6)
                        {
                            sb.Append((char)(accum + 63));
                            accum = 0;
                            nb = 0;
                        }
                    }

                    k = 0;
                }
            }
        }

        // Add any remaining bits
        if (nb > 0)
        {
            accum <<= (6 - nb);
            sb.Append((char)(accum + 63));
        }

        return sb.ToString();
    }

    public static Graph? Deserialize(string sparse6)
    {
        if (string.IsNullOrEmpty(sparse6) || sparse6[0] != ':')
            throw new ArgumentException("Invalid Sparse6 format.");

        int index = 1;

        // Decode the number of nodes
        int n;
        index = ReadInteger(sparse6, index, out n);

        if (n < 0)
            throw new ArgumentException("Invalid number of nodes.");

        var builder = new GraphBuilder(n);

        // Load all bits from the string
        List<int> bits = new List<int>();
        for (; index < sparse6.Length; index++)
        {
            int c = sparse6[index] - 63;
            for (int i = 5; i >= 0; i--)
            {
                bits.Add((c >> i) & 1);
            }
        }

        int bitPtr = 0;
        int last = -1;
        int k = 0; // Number of consecutive '1' bits

        for (int i = 0; i < n; i++)
        {
            while (bitPtr < bits.Count)
            {
                int b = bits[bitPtr++];

                if (b == 1)
                {
                    k++;
                }
                else
                {
                    int d = 0;
                    for (int s = 0; s < k; s++)
                    {
                        if (bitPtr >= bits.Count)
                            throw new ArgumentException("Invalid Sparse6 format: unexpected end of data.");

                        d = (d << 1) | bits[bitPtr++];
                    }

                    d += (1 << k) - 1;
                    last += d + 1;
                    if (last >= n)
                        break;
                    builder.SetEdge(i, last, true);
                    k = 0;
                }
            }

            last = i;
        }

        return builder.Build();
    }

    // Helper method to encode an integer into the Sparse6 format
    private static void AppendInteger(StringBuilder sb, int value)
    {
        if (value == 0)
        {
            sb.Append((char)(value + 63));
            return;
        }

        List<int> bytes = new List<int>();
        while (value > 0)
        {
            bytes.Add((value & 0x3F) + 63);
            value >>= 6;
        }

        for (int i = bytes.Count - 1; i >= 0; i--)
        {
            sb.Append((char)bytes[i]);
        }
    }

    // Helper method to decode an integer from the Sparse6 format
    private static int ReadInteger(string s, int index, out int value)
    {
        value = 0;
        while (index < s.Length)
        {
            int c = s[index] - 63;
            if (c < 0 || c > 63)
                throw new ArgumentException("Invalid character in Sparse6 format.");

            value = (value << 6) | c;
            index++;
            if (c < 63)
                break;
        }

        return index;
    }
}