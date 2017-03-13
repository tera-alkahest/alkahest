using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Alkahest.Core.IO;
using Alkahest.Core.Net.Protocol;

namespace Alkahest.Analyzer.Analysis
{
    static class PacketAnalysis
    {
        public static IEnumerable<PotentialString> FindStrings(
            byte[] payload)
        {
            using (var reader = new TeraBinaryReader(payload))
            {
                for (var i = 0; i < reader.Length; i++)
                {
                    reader.Position = i;

                    if (!reader.CanRead(sizeof(ushort)))
                        break;

                    var offsetPos = reader.Position;
                    var offset = reader.ReadUInt16() - PacketHeader.HeaderSize;

                    if (offset < 0 || offset < offsetPos + sizeof(ushort) ||
                        offset > reader.Length - sizeof(char))
                        continue;

                    reader.Position = offset;

                    string str;

                    try
                    {
                        str = reader.ReadString();

                        if (offset < offsetPos &&
                            reader.Position > offsetPos)
                            continue;

                        TeraBinaryReader.Encoding.GetString(
                            TeraBinaryReader.Encoding.GetBytes(str));
                    }
                    catch (Exception e) when (IsStringException(e))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(str))
                        continue;

                    var hasBadChars = str.Any(c =>
                    {
                        var cat = char.GetUnicodeCategory(c);

                        return cat == UnicodeCategory.Control ||
                            cat == UnicodeCategory.Format ||
                            cat == UnicodeCategory.OtherNotAssigned ||
                            cat == UnicodeCategory.PrivateUse ||
                            cat == UnicodeCategory.Surrogate;
                    });

                    if (!hasBadChars)
                        yield return new PotentialString(offsetPos,
                            (ushort)offset, str);
                }
            }
        }

        static bool IsStringException(Exception e)
        {
            return e is EndOfStreamException || e is ArgumentException;
        }

        public static IEnumerable<PotentialArray> FindArrays(
            byte[] payload)
        {
            using (var reader = new TeraBinaryReader(payload))
            {
                for (var i = 0; i < reader.Length; i++)
                {
                    reader.Position = i;

                    if (!reader.CanRead(sizeof(ushort) * 2))
                        break;

                    var countPos = reader.Position;
                    var count = reader.ReadUInt16();
                    var offsetPos = reader.Position;
                    var offset = reader.ReadUInt16() -
                        PacketHeader.HeaderSize;

                    if (count == 0 ||
                        (count * (sizeof(ushort) * 2 + sizeof(byte)) +
                        sizeof(ushort) * 2) > reader.Length)
                        continue;

                    var positions = new HashSet<int>
                    {
                        countPos,
                        countPos + sizeof(byte),
                        offsetPos,
                        offsetPos + sizeof(byte)
                    };

                    var elems = new List<PotentialArrayElement>();
                    var good = true;
                    var last = offsetPos;
                    var next = offset;

                    while (next != -PacketHeader.HeaderSize)
                    {
                        if (!(good = next >= 0 && next > last &&
                            next <= reader.Length - sizeof(ushort) * 2 - sizeof(byte)))
                            break;

                        last = next + sizeof(ushort) * 2;

                        reader.Position = next;

                        var herePos = reader.Position;
                        var here = reader.ReadUInt16() -
                            PacketHeader.HeaderSize;

                        if (!(good = here == herePos && here == next &&
                            positions.Add(herePos) &&
                            positions.Add(herePos + sizeof(byte))))
                            break;

                        var nextPos = reader.Position;
                        next = reader.ReadUInt16() -
                            PacketHeader.HeaderSize;

                        if (!(good = positions.Add(nextPos) &&
                            positions.Add(nextPos + sizeof(byte))))
                            break;

                        elems.Add(new PotentialArrayElement((ushort)here,
                            nextPos, (ushort)(next == -PacketHeader.HeaderSize ?
                            0 : next)));
                    }

                    if (good && elems.Count == count)
                        yield return new PotentialArray(countPos, count,
                            offsetPos, (ushort)offset, elems);
                }
            }
        }
    }
}
