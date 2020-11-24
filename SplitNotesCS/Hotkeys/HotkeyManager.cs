using LowLevelHooking;
using System;
using System.Collections.Generic;

namespace SplitNotesCS.Hotkeys
{
    public class HotkeyManager
    {
        public GlobalKeyboardHook Hook { get; private set; }
        public List<Hotkey> hotkeys = new List<Hotkey> { };
        private Action<int> activeKeySelect;

        public bool enabled = false;
        
        public HotkeyManager()
        {
            this.Hook = new GlobalKeyboardHook();
        }

        public void Enable(List<Hotkey> hotkeys)
        {
            // Ignore hotkeys that are defined as escape
            List<Hotkey> activeKeys = new List<Hotkey>();
            foreach (Hotkey hotkey in hotkeys)
            {
                if (hotkey.keycode != VirtualKey.Escape) { activeKeys.Add(hotkey); }
            }

            this.hotkeys = activeKeys;
            this.Hook.KeyDownOrUp += this.KeyboardListener;
        }

        public void Disable()
        {
            // Clear List
            this.hotkeys.Clear();
            this.Hook.KeyDownOrUp -= this.KeyboardListener;
        }
        
        /// <summary>
        /// This function does the actual listening for keys in the main program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyboardListener(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (!e.IsUp)  // Activate only on keydown
            {
                foreach (Hotkey hotkey in this.hotkeys)
                {
                    if (hotkey.keycode == e.KeyCode)
                    {
                        hotkey.callback();
                        break;  // Only perform one action
                    }
                }
            }
        }

        public void ChooseKey(Action<int> callback) 
        {
            this.activeKeySelect = callback;
            this.Hook.KeyDownOrUp += this.KeyFinder;
        }

        /// <summary>
        /// This function is used for selecting the keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyFinder(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (!e.IsUp)  // Activate only on keydown
            {
                this.activeKeySelect((int)e.KeyCode);
                this.Hook.KeyDownOrUp -= this.KeyFinder;
            }
            
        }

        public void Cleanup()
        {
            this.Disable();
            this.Hook.Dispose();
        }

    }
}
