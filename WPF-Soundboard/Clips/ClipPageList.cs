using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using WPF_Soundboard.Hotkeys;

namespace WPF_Soundboard.Clips
{
    [JsonObject]
    public class ClipPageList : IList<ClipPage>
    {

        public event EventHandler OnChanged;
        public event EventHandler<GlobalHotkeyEventArgs> OnGlobalHotkeyPressed;
        public event EventHandler OnStopHotkeyPressed;

        [JsonProperty]
        private List<ClipPage> ClipPages { get; }
        [JsonProperty]
        public GlobalHotkeyList GlobalHotkeyList { get; }
        [JsonProperty]
        public bool AutoStart { get => autoStart; set { autoStart = value; CallOnChanged(); } }

        private HotkeyInfo stopHotkeyInfo;

        private Hotkey stopHotkey;
        private bool autoStart;

        [JsonIgnore]
        public int Count => ClipPages.Count;
        [JsonIgnore]
        public bool IsReadOnly => false;
        [JsonProperty]
        public HotkeyInfo StopHotkeyInfo { get => stopHotkeyInfo; set { stopHotkeyInfo = value; InitializeHotkey(); CallOnChanged(); } }

        ClipPage IList<ClipPage>.this[int index] { get => ClipPages[index]; set { ClipPages[index] = value; value.OnChanged += Value_OnChanged; CallOnChanged(); } }



        private void Value_OnChanged(object sender, EventArgs e) => CallOnChanged();

        public ClipPage this[int page] => ClipPages[page];


        public ClipPageList()
        {
            ClipPages = new List<ClipPage>();
            GlobalHotkeyList = new GlobalHotkeyList();
        }

        [JsonConstructor]
        public ClipPageList(List<ClipPage> clipPages, GlobalHotkeyList globalHotkeyList, HotkeyInfo stopHotkeyInfo)
        {
            ClipPages = new List<ClipPage>();
            ClipPages.AddRange(clipPages);
            foreach (ClipPage clipPage in clipPages)
            {
                clipPage.OnChanged += Value_OnChanged;
            }
            this.stopHotkeyInfo = stopHotkeyInfo;
            InitializeHotkey();
            GlobalHotkeyList = globalHotkeyList ?? new GlobalHotkeyList();
            GlobalHotkeyList.OnHotkeyPressed += GlobalHotkeyList_OnHotkeyPressed;
            GlobalHotkeyList.OnChanged += Value_OnChanged;
        }


        private void InitializeHotkey()
        {
            if (stopHotkey != null)
            {
                stopHotkey.Dispose();
                stopHotkey = null;
            }

            if (stopHotkeyInfo.Enabled)
            {
                stopHotkey = new Hotkey(stopHotkeyInfo.ModifierKeys, stopHotkeyInfo.Key);
                stopHotkey.Register();
                stopHotkey.Pressed += Hotkey_Pressed;
            }
        }

        private void Hotkey_Pressed(object sender, EventArgs e) => OnStopHotkeyPressed?.Invoke(this, EventArgs.Empty);
        private void GlobalHotkeyList_OnHotkeyPressed(object sender, GlobalHotkeyEventArgs e) => OnGlobalHotkeyPressed?.Invoke(this, e);

        public void Add(ClipPage page)
        {

            ClipPages.Add(page);
            page.OnChanged += Value_OnChanged;
            CallOnChanged();
        }
        public void Remove(ClipPage page)
        {
            ClipPages.Remove(page);
            CallOnChanged();
        }
        public int IndexOf(ClipPage item) => ((IList<ClipPage>)ClipPages).IndexOf(item);
        public void Insert(int index, ClipPage item)
        {
            item.OnChanged += Value_OnChanged;
            ((IList<ClipPage>)ClipPages).Insert(index, item);
            CallOnChanged();
        }
        public void RemoveAt(int index)
        {
            ((IList<ClipPage>)ClipPages).RemoveAt(index);
            CallOnChanged();
        }

        public void Clear() => ((IList<ClipPage>)ClipPages).Clear();
        public bool Contains(ClipPage item) => ((IList<ClipPage>)ClipPages).Contains(item);
        public void CopyTo(ClipPage[] array, int arrayIndex) => ((IList<ClipPage>)ClipPages).CopyTo(array, arrayIndex);
        bool ICollection<ClipPage>.Remove(ClipPage item)
        {
            bool ret = ((IList<ClipPage>)ClipPages).Remove(item);
            CallOnChanged();
            return ret;
        }

        public IEnumerator<ClipPage> GetEnumerator() => ((IList<ClipPage>)ClipPages).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IList<ClipPage>)ClipPages).GetEnumerator();



        private void CallOnChanged() => OnChanged?.Invoke(this, null);
    }
}
