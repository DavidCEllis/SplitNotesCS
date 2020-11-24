using LowLevelHooking;
using System;

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
