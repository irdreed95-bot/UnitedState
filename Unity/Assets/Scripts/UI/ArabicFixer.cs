using System;
using System.Text;
using UnityEngine;
namespace CorruptStateRP.UI {
public static class ArabicFixer {
 struct Forms { public char Isolated,Final,Initial,Medial; public Forms(char i,char f,char n,char m){Isolated=i;Final=f;Initial=n;Medial=m;} }
 static readonly char[] chars="اأإآبتثجحخدذرزسشصضطظعغفقكلمنهويىةؤئء".ToCharArray();
 static readonly Forms[] forms={
  new Forms('\uFE8D','\uFE8E','\uFE8D','\uFE8E'),new Forms('\uFE83','\uFE84','\uFE83','\uFE84'),new Forms('\uFE87','\uFE88','\uFE87','\uFE88'),new Forms('\uFE81','\uFE82','\uFE81','\uFE82'),
  new Forms('\uFE8F','\uFE90','\uFE91','\uFE92'),new Forms('\uFE95','\uFE96','\uFE97','\uFE98'),new Forms('\uFE99','\uFE9A','\uFE9B','\uFE9C'),new Forms('\uFE9D','\uFE9E','\uFE9F','\uFEA0'),new Forms('\uFEA1','\uFEA2','\uFEA3','\uFEA4'),new Forms('\uFEA5','\uFEA6','\uFEA7','\uFEA8'),
  new Forms('\uFEA9','\uFEAA','\uFEA9','\uFEAA'),new Forms('\uFEAB','\uFEAC','\uFEAB','\uFEAC'),new Forms('\uFEAD','\uFEAE','\uFEAD','\uFEAE'),new Forms('\uFEAF','\uFEB0','\uFEAF','\uFEB0'),
  new Forms('\uFEB1','\uFEB2','\uFEB3','\uFEB4'),new Forms('\uFEB5','\uFEB6','\uFEB7','\uFEB8'),new Forms('\uFEB9','\uFEBA','\uFEBB','\uFEBC'),new Forms('\uFEBD','\uFEBE','\uFEBF','\uFEC0'),new Forms('\uFEC1','\uFEC2','\uFEC3','\uFEC4'),new Forms('\uFEC5','\uFEC6','\uFEC7','\uFEC8'),new Forms('\uFEC9','\uFECA','\uFECB','\uFECC'),new Forms('\uFECD','\uFECE','\uFECF','\uFED0'),new Forms('\uFED1','\uFED2','\uFED3','\uFED4'),new Forms('\uFED5','\uFED6','\uFED7','\uFED8'),new Forms('\uFED9','\uFEDA','\uFEDB','\uFEDC'),new Forms('\uFEDD','\uFEDE','\uFEDF','\uFEE0'),new Forms('\uFEE1','\uFEE2','\uFEE3','\uFEE4'),new Forms('\uFEE5','\uFEE6','\uFEE7','\uFEE8'),new Forms('\uFEE9','\uFEEA','\uFEEB','\uFEEC'),new Forms('\uFEED','\uFEEE','\uFEED','\uFEEE'),new Forms('\uFEF1','\uFEF2','\uFEF3','\uFEF4'),new Forms('\uFEEF','\uFEF0','\uFEEF','\uFEF0'),new Forms('\uFE93','\uFE94','\uFE93','\uFE94'),new Forms('\uFE85','\uFE86','\uFE85','\uFE86'),new Forms('\uFE89','\uFE8A','\uFE8B','\uFE8C'),new Forms('\uFE80','\uFE80','\uFE80','\uFE80')
 };
 static bool CanJoinNext(char c){int i=Array.IndexOf(chars,c);if(i<0)return false;return "اأإآدذرزوىةؤء".IndexOf(c)<0;}
 static bool CanJoinPrevious(char c){return Array.IndexOf(chars,c)>=0&&c!='ء';}
 public static string Fix(string input){
  if(string.IsNullOrEmpty(input))return input;
  var shaped=new StringBuilder(input.Length);
  for(int i=0;i<input.Length;i++){
   char c=input[i];int ix=Array.IndexOf(chars,c);if(ix<0){shaped.Append(c);continue;}
   bool prev=i>0&&CanJoinNext(input[i-1])&&CanJoinPrevious(c);
   bool next=i<input.Length-1&&CanJoinNext(c)&&CanJoinPrevious(input[i+1]);
   var f=forms[ix];shaped.Append(prev&&next?f.Medial:prev?f.Final:next?f.Initial:f.Isolated);
  }
  var s=shaped.ToString();var reversed=new StringBuilder(s.Length);for(int i=s.Length-1;i>=0;i--)reversed.Append(s[i]);
  // Restore Latin/numeric runs so mixed Arabic/English labels keep their order.
  var charsOut=reversed.ToString().ToCharArray();int start=0;
  while(start<charsOut.Length){if(IsLtr(charsOut[start])){int end=start+1;while(end<charsOut.Length&&IsLtr(charsOut[end]))end++;Array.Reverse(charsOut,start,end-start);start=end;}else start++;}
  return new string(charsOut);
 }
 static bool IsLtr(char c)=>c >= ' ' && c <= '~';
}
}