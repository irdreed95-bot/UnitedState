using System.Text;
using UnityEngine;

namespace CorruptStateRP.UI {
public static class ArabicFixer {
 static readonly char[] chars={'ا','أ','إ','آ','ب','ت','ث','ج','ح','خ','د','ذ','ر','ز','س','ش','ص','ض','ط','ظ','ع','غ','ف','ق','ك','ل','م','ن','ه','و','ي','ى','ة','ؤ','ئ','ء'};
 static readonly string[] forms={"ﺍﺎ","ﺃﺃ","ﺇﺇ","ﺁﺂ","ﺏﺑﺒﺒ","ﺕﺗﺘﺖ","ﺙﺛﺜﺚ","ﺝﺟﺠﺞ","ﺡﺣﺤﺢ","ﺥﺧﺨﺦ","ﺩﺩ","ﺫﺫ","ﺭﺭ","ﺯﺯ","ﺱﺳﺴﺲ","ﺵﺷﺸﺶ","ﺹﺻﺼﺺ","ﺽﺿﻀﻀ","ﻁﻃﻄﻂ","ﻅﻇﻈﻆ","ﻉﻋﻌﻊ","ﻍﻏﻐﻎ","ﻑﻓﻔﻒ","ﻕﻗﻘﻖ","ﻙﻛﻜﻚ","ﻝﻟﻠﻞ","ﻡﻣﻤﻢ","ﻥﻧﻨﻦ","ﻩﻫﻬﻪ","ﻭﻭ","ﻯﻳﻴﻰ","ﻯﻯ","ﺓﺔ","ﺅﺅ","ﺉﺋﺌﺊ","ﺀﺀ"};
 public static string Fix(string input){
  if(string.IsNullOrEmpty(input))return input;
  var a=input.ToCharArray();var outp=new StringBuilder();
  for(int i=0;i<a.Length;i++){
   char ch=a[i];int idx=System.Array.IndexOf(chars,ch);
   if(idx<0){outp.Append(ch);continue;}
   bool prev=i>0&&Connects(a[i-1]);bool next=i<a.Length-1&&Connects(a[i+1]);
   string f=forms[idx];char shaped;
   if(f.Length==2) shaped=prev?f[1]:f[0];
   else shaped=prev&&next?f[3]:prev?f[2]:next?f[1]:f[0];
   outp.Append(shaped);
  }
  var s=outp.ToString();
  var rev=new StringBuilder();for(int i=s.Length-1;i>=0;i--)rev.Append(s[i]);
  return rev.ToString();
 }
 static bool Connects(char c){return System.Array.IndexOf(chars,c)>=0&&c!='ا'&&c!='أ'&&c!='إ'&&c!='آ'&&c!='د'&&c!='ذ'&&c!='ر'&&c!='ز'&&c!='و'&&c!='ؤ'&&c!='ء';}
}
}