using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SheetData.Editor.DownLoader
{
    internal class SheetLoader
    {
        public static async Task Load(string sheetID, string sheetName)
        {
            // 1. 요청할 URL 생성
            string targetUrl =  $"https://docs.google.com/spreadsheets/d/{sheetID}/gviz/tq?tqx=out:csv&sheet={sheetName}";
            
            // 2. UnityWebRequest를 사용한 비동기 요청
            using (UnityWebRequest webRequest = UnityWebRequest.Get(targetUrl))
            {
                // 요청이 완료될 때까지 대기
                await webRequest.SendWebRequest();
                // 3. 오류 처리
                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"GoogleSheet Load Error: {webRequest.error}");
                }
                else
                {
                    // 4. 데이터 수신 및 파싱 시작
                    string tsvData = webRequest.downloadHandler.text;
                    //return SheetData
                }
            }
        }
        
        /// <summary>
        /// 구글 스프레드시트안에 포함된 모든 시트를 가져옵니다.
        /// </summary>
        /// <param name="sheetID"></param>
        /// <returns></returns>
        internal static async Task<List<string>> GetSheetNames(string sheetID)
        {
            List<string> result = new();
            string accessUrl = $"https://docs.google.com/spreadsheets/d/{sheetID}/edit";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(accessUrl))
            {
                // 브라우저처럼 위장하여 요청 (선택 사항이나 때때로 필요함)
                webRequest.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                await webRequest.SendWebRequest();
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed GetSheetNames : {webRequest.error}");
                    return result;
                }
                string htmlContent = webRequest.downloadHandler.text;
                
                // Google Sheets HTML 소스 내에서 시트 정보를 포함하는 패턴을 찾습니다.
                string replacement = "<div class=\"goog-inline-block docs-sheet-tab-caption\">";
                string pattern = @"<div class=""goog-inline-block docs-sheet-tab-caption"">(.*?)<\/div>";
                MatchCollection matches = Regex.Matches(htmlContent, pattern);
                if (matches.Count == 0)
                {
                    Debug.LogError("Couldn't find the sheet list pattern in the HTML. The Google Sheets web structure may have changed.");
                    return result;
                }
                foreach (Match match in matches)
                    result.Add(match.Groups[1].Value);
            }
            return result;
        }
    }
}