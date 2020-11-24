using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LowLevelHooking;

namespace SplitNotesCS.Hotkeys
{
    public class Hotkey
    {
        public VirtualKey keycode;
        public Action callback;

        public Hotkey(int keycode, Action callback)
        {
            this.keycode = (VirtualKey)keycode;
            this.callback = callback;
        }

    }
}
