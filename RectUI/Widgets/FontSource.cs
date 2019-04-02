using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RectUI.Widgets
{
    public struct FontFaceName
    {
        public string FamilylName;
        public string FaceName;

        public FontFaceName(string family, string face)
        {
            FamilylName = family;
            FaceName = face;
        }

        public override string ToString()
        {
            return $"{FamilylName}: {FaceName}";
        }
    }

    public class FontSource : IListSource<FontFaceName>
    {
        List<FontFaceName> m_list;

        public FontFaceName this[int index] => m_list[index];

        public int Count => m_list.Count;

        public event Action Updated;

        IEnumerable<FontFaceName> EnumerateFaces()
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
                                    yield return new FontFaceName(fontFamilyName, faceName);
                                }
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

        public ListItemRegion<FontFaceName> CreateItem()
        {
            return new ListItemRegion<FontFaceName>();
        }
    }
}
