using UnityEngine;
using System.Collections.Generic;
using BrokeProtocolClient.modules;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;

namespace BrokeProtocolClient.UI
{
    abstract class GuiWindow
    {
        protected static int rollingIndex;

        public int windowIndex;
        public Rect windowSpace;
        public string windowTitle;

        public float controlIndex;

        public int columns = 1;
        public int currentColumn = 0;

        public int rows = 0;
        public int currentRow = 0;

        public int headerPad = 20;
        public int pad = 5;
        public int controlHeight = 20;
        public int controlWidth = 200;

        public GuiWindow(string name)
        {
            this.windowTitle = name;
            windowSpace = new Rect(0, 0, (controlWidth + pad) * columns + pad, (controlHeight + pad) * controlIndex + headerPad);
        }

        protected abstract void init();
        protected void ResizeWindowToFitControls()
        {
            windowSpace.width = (controlWidth + pad) * columns + pad;
            windowSpace.height = (controlHeight + pad) * controlIndex + headerPad;
        }

        protected abstract void WindowCore();

        protected abstract void WindowWrapper(int id);

        public abstract void onRender();

        public abstract void onUpdate();
    }
}
