using System;
using System.Collections.Generic;
using System.Text;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard.Gui.Controls
{
    public class ClipButtonDragEventArgs : EventArgs
    {
        public ClipButton Source { get; set; }
        public (int x, int y) SourceCoordinates { get; set; }
        public ClipButton Destination { get; set; }
        public (int x, int y) DestinationCoordinates { get; set; }
    }
}
