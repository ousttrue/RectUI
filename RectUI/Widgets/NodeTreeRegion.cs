using RectUI.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RectUI.Widgets
{
    public class NodeTreeRegion : RectRegion, ISingleSelector<Node>
    {
        public int? SelectedSourceIndex => throw new NotImplementedException();

        public Node Selected => throw new NotImplementedException();

        public event Action SelectionChanged;
    }
}
