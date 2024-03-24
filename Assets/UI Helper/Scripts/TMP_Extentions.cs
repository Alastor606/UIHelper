namespace UIHelper
{
    using System.Collections;
    using TMPro;
    using UnityEngine;

    public static class TMP_Extentions
    {
        public static TMP_Text Write(this TMP_Text self,string message, float delay, bool withClear = true)
        {
            if (withClear) self.text = string.Empty;
            self.StartCoroutine(WriteCoroutine(self, message, delay));
            return self;
        }

        public static TMP_Text Write(this CustomButton self, string message, float delay, bool withClear = true)
        {
            if (withClear) self.text = string.Empty;
            self.StartCoroutine(WriteCoroutine(self.TMP, message, delay));
            return self.TMP;
        }

        public static TMP_Text Write(this RadioButton self, string message, float delay, bool withClear = true)
        {
            if (withClear) self.text = string.Empty;
            self.StartCoroutine(WriteCoroutine(self.TMP, message, delay));
            return self.TMP;
        }

        public static void Clear(this TMP_Text self, float delay = 0) => self.StartCoroutine(ClearCoroutine(self, delay));
        public static void Clear(this CustomButton self, float delay = 0) => self.StartCoroutine(ClearCoroutine(self.TMP, delay));
        public static void Clear(this TMP_InputField self, float delay = 0) => self.StartCoroutine(ClearCoroutine(self.textComponent, delay));
        public static void Clear(this RadioButton self, float delay = 0) => self.StartCoroutine(ClearCoroutine(self.TMP, delay));

        private static IEnumerator WriteCoroutine(TMP_Text tmp, string message, float delay)
        {
            foreach (var item in message)
            {
                yield return new WaitForSeconds(delay);
                tmp.text += item;
            }
        }

        public static IEnumerator ClearCoroutine(TMP_Text tmp, float delay)
        {
            yield return new WaitForSeconds(delay);
            tmp.text = string.Empty;
        }
    }
}

