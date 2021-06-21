using Autodesk.Navisworks.Api.Plugins;
using Autodesk.Navisworks.Api;

namespace NVSXBOX
{
    [Plugin("NVSXBOX.CmdInput", "ATLAS")]
    public class CmdInput : InputPlugin
    {
        public override bool MouseDown(View view, KeyModifiers modifiers, ushort button, int x, int y, double timeOffset)
        {
            bool doubleClick = modifiers.HasFlag(KeyModifiers.DoubleClick);//Determine if trigered by one of the following: WM_LBUTTONDBLCLK, WM_MBUTTONDBLCLK or WM_RBUTTONDBLCLK.
            PickItemResult itemResult = view.PickItemFromPoint(x, y);
            if (itemResult != null)
            {
                ModelItem modelItem = itemResult.ModelItem;
                //Debug.WriteLine(modelItem.ClassDisplayName);
            }
            return false;
        }

        public override bool MouseUp(View view, KeyModifiers modifiers, ushort button, int x, int y, double timeOffset)
        {
            return false;
        }

        public override bool ContextMenu(View view, int x, int y)
        {
            return false;
        }

        public override bool KeyDown(View view, KeyModifiers modifier, ushort key, double timeOffset)
        {
            //Debug.WriteLine(modifier.ToString() + ", " + key);
            return false;
        }
    }
}