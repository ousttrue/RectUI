using RectUI.Graphics;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RectUI.Widgets
{
    public class FontSource : IListSource<FontInfo>
    {
        List<FontInfo> m_list;

        public FontInfo this[int index] => m_list[index];

        public int Count => m_list.Count;

        public event Action Updated;

        IEnumerable<FontInfo> EnumerateFaces()
        {
            using (var factory = new Factory())
            {
                using (var fc = factory.GetSystemFontCollection(false))
                {
                    for (int i = 0; i < fc.FontFamilyCount; ++i)
                    {
                        var fontFamily = fc.GetFontFamily(i);
                        using (var fontFamilylNames = fontFamily.FamilyNames)
                        {
                            var fontFamilyName = fontFamilylNames.GetString(0);

                            for (int j = 0; j < fontFamily.FontCount; ++j)
                            {
                                var font = fontFamily.GetFont(j);
                                using (var faceNames = font.FaceNames)
                                {
                                    var faceName = font.FaceNames.GetString(0);
                                    yield return new FontInfo(fontFamilyName, faceName);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        public FontSource()
        {
            m_list = EnumerateFaces().ToList();
        }

        public ListItemRegion<FontInfo> CreateItem()
        {
            return new ListItemRegion<FontInfo>();
        }
    }
}
