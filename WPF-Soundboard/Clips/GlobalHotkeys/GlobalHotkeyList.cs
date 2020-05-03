using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace WPF_Soundboard.Clips
{
    [JsonArray]
    public class GlobalHotkeyList : IDictionary<(int x, int y), GlobalHotkey>
    {
        public event EventHandler OnChanged;
        public event EventHandler<GlobalHotkeyEventArgs> OnHotkeyPressed;

        [JsonProperty]
        public Dictionary<(int x, int y), GlobalHotkey> globalHotkeys { get; private set; }

        public GlobalHotkey this[int x, int y] => ContainsKey((x, y)) ? globalHotkeys[(x, y)] : null;

        public GlobalHotkeyList()
        {
            globalHotkeys = new Dictionary<(int x, int y), GlobalHotkey>();
        }

        [JsonConstructor]
        public GlobalHotkeyList(IEnumerable<KeyValuePair<(int x, int y), GlobalHotkey>> globalHotkeys)
        {
            this.globalHotkeys = new Dictionary<(int x, int y), GlobalHotkey>();
            foreach (KeyValuePair<(int x, int y), GlobalHotkey> pair in globalHotkeys)
            {
                this.globalHotkeys.Add(pair.Key, pair.Value);
                pair.Value.OnChanged += Value_OnChanged;
                pair.Value.OnHotkeyPressed += GlobalHotkey_OnHotkeyPressed;
            }
        }

        private void Value_OnChanged(object sender, EventArgs e) => CallOnChanged();
        private void CallOnChanged() => OnChanged?.Invoke(this, EventArgs.Empty);

        public void AddHotkey(int x, int y, HotkeyInfo hotkeyInfo)
        {
            if (ContainsKey((x, y)))
            {
                globalHotkeys[(x, y)].GlobalHotkeyInfo = new GlobalHotkeyInfo()
                {
                    HotkeyInfo = hotkeyInfo,
                    x = x,
                    y = y
                };
            }
            else
            {
                GlobalHotkey globalHotkey = new GlobalHotkey(new GlobalHotkeyInfo()
                {
                    HotkeyInfo = hotkeyInfo,
                    x = x,
                    y = y
                });
                globalHotkey.OnChanged += Value_OnChanged;
                globalHotkey.OnHotkeyPressed += GlobalHotkey_OnHotkeyPressed;
                globalHotkeys.Add((x, y), globalHotkey);
            }
            CallOnChanged();
        }

        private void GlobalHotkey_OnHotkeyPressed(object sender, GlobalHotkeyEventArgs e) => OnHotkeyPressed?.Invoke(this, e);

        #region IDictionary implementation
        public GlobalHotkey this[(int x, int y) key] { get => ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys)[key]; set => ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys)[key] = value; }

        [JsonIgnore]
        public ICollection<(int x, int y)> Keys => ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).Keys;
        [JsonIgnore]
        public ICollection<GlobalHotkey> Values => ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).Values;
        [JsonIgnore]
        public int Count => ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).Count;
        [JsonIgnore]
        public bool IsReadOnly => ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).IsReadOnly;

        public void Add((int x, int y) key, GlobalHotkey value)
        {
            ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).Add(key, value);
            value.OnChanged += Value_OnChanged;
            value.OnHotkeyPressed += GlobalHotkey_OnHotkeyPressed;
            CallOnChanged();
        }
        public void Add(int x, int y, GlobalHotkey value)
        {
            ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).Add((x, y), value);
            value.OnChanged += Value_OnChanged;
            value.OnHotkeyPressed += GlobalHotkey_OnHotkeyPressed;
            CallOnChanged();
        }

        public void Add(KeyValuePair<(int x, int y), GlobalHotkey> item)
        {
            ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).Add(item);
            item.Value.OnChanged += Value_OnChanged;
            item.Value.OnHotkeyPressed += GlobalHotkey_OnHotkeyPressed;
            CallOnChanged();
        }

        public void Clear()
        {
            ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).Clear();
            CallOnChanged();
        }

        public bool Contains(KeyValuePair<(int x, int y), GlobalHotkey> item) => ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).Contains(item);
        public bool ContainsKey((int x, int y) key) => ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).ContainsKey(key);
        public void CopyTo(KeyValuePair<(int x, int y), GlobalHotkey>[] array, int arrayIndex) => ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).CopyTo(array, arrayIndex);
        public IEnumerator<KeyValuePair<(int x, int y), GlobalHotkey>> GetEnumerator() => ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).GetEnumerator();
        public bool Remove((int x, int y) key)
        {
            if (globalHotkeys.ContainsKey(key))
                globalHotkeys[key].Dispose();
            bool ret = ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).Remove(key);
            CallOnChanged();
            return ret;
        }

        public bool Remove(KeyValuePair<(int x, int y), GlobalHotkey> item)
        {
            item.Value?.Dispose();
            bool ret = ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).Remove(item);
            CallOnChanged();
            return ret;
        }

        public bool TryGetValue((int x, int y) key, [MaybeNullWhen(false)] out GlobalHotkey value) => ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<(int x, int y), GlobalHotkey>)globalHotkeys).GetEnumerator();
        #endregion
    }
}
