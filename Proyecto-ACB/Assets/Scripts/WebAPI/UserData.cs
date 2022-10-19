using System;
using UnityEngine;

namespace WebAPI
{
    [Serializable]
    public class UserData
    {
#if !UNITY_EDITOR
        public string user = "null";
        [TextArea]
        public string accessToken= "null";
        [TextArea]
        public string refreshToken = "null";
#endif
#if UNITY_EDITOR
        [SerializeField] [Tooltip("identificacion del usuario autenticado")]
        public string user = "eae88992-86a2-41b6-a912-f13bc7a5a255";
        [TextArea][SerializeField] [Tooltip("accessToken para la autenticacion del usuario")]
        public string accessToken= "eyJraWQiOiJtc2pNc2NYdk0rWjA4UXZVa3VTRFZiWGRFYmZBd0lRSWVqeEZtOGE1NEdnPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiJlYWU4ODk5Mi04NmEyLTQxYjYtYTkxMi1mMTNiYzdhNWEyNTUiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMV9GWm1aRnpKR0wiLCJjbGllbnRfaWQiOiIzaDNkN25qZ3F1aHJqNWhqaTB0b25rZG5rayIsIm9yaWdpbl9qdGkiOiJiZDQxYjllMS1iYWZiLTQxYjItYTczYS0zZmExMDIwOWUwZWYiLCJldmVudF9pZCI6IjQ4ZDAxYTIwLThmOTQtNDFlMy05MjM5LTY1ZjRlOWQ5MzcyNCIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4iLCJhdXRoX3RpbWUiOjE2NjYxMzU2ODYsImV4cCI6MTY2NjEzOTI4NiwiaWF0IjoxNjY2MTM1Njg2LCJqdGkiOiJkMjJlZDVhZS0zZDhhLTQ4ZWMtOWFiNi1kODI0MTdlNDk2MjYiLCJ1c2VybmFtZSI6ImVhZTg4OTkyLTg2YTItNDFiNi1hOTEyLWYxM2JjN2E1YTI1NSJ9.JwRPXmV7JJv0YAqDyExyd-tbpMHiWeNATp0w7UHNk0e3GH2XUcTSpB7QRPKfunPrfRWje84XzkzBbXkNnaTYVQUpKgZpkWviygH6CYqR4Yc7i2q05q743SvKR5UhbM1DjeKEmTVM7uijU_DlHN3Tpe4ZGcaDzoYH-6-J471K3HSfZtW58hbyCARVdafYUvpeKgjH9Ila43MHKMZYahcxFUJ-KFB1VYweiM_A37lYDGjvQ8VfKL7-qPyfmg1I1ouzs2cFpmlp0DJ1dx7DMa5iAc8hyYUI0Xw3HAaMQJ-AtnfkMsK8OpG9DyqwNHBI7aXlQiH2I3-kb0ccPklIRMclhA";
        [TextArea]
        [SerializeField]
        [Tooltip("refreshToken para la autenticacion del usuario")]
        public string refreshToken = "eyJjdHkiOiJKV1QiLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiUlNBLU9BRVAifQ.mCV6Vy_dgasvuEEB8aq-TtZ9LJp_y2ATS_harELlrHU0fUFiMc2egRg5Jl3UMVRCk-d4-jRPyGc1PWRu9EXH5JN6HHjiNOPI25eymda7XzVYr4l2m8NXSpy8MQjzGKMk2PnhT2NUVXwEVufvEK54uP-DBTyoPNKr3MXc5IGxTRasl4KNRcnsa20-ESkhPE4OSLmc7YFtppEL_4AMQlsvaBmKylbkFxXBCVDaa70XN32WQCkrAMyu_KhD2MUveHTWrdJux5Ml3ZkbDgnuOtFixEFsLeaiGWdHNLim-Jwkq3KhI3u2_tsLn7LLic7u_jnyza5cJvzXOyLtMUZFZcDE1w.leo-HGrpVI5C5v0K.BDPwGKfDXA-zm3rew6awkHUMY2JKua3pkcggywrEyWAweUi0KUqQMbPvCqv7ggU-uEsLOqeN6ow6H_fOsUSKreEJU9skQzRUzIbU7sXPzJircxv2LjuLtF0Gyo4sdQhZoiqeuOhUrWgSjGlMyPq-Yn1-4QrtDKZAKn9g3ZcQgiLCfwuREihDEbBh2smhOA5bOQU00qiHKskXXGtBEiNcYKUvCeXRK_OOjk00IOFPbvnoDWxUdEOTTkhJQ1w7JEajGCn9JFo7q8o8WbnLM9W_OleyrDQLL5XnddbCC33ECnGdp7DWL2w6naUH6ijGJQfM5ATxV1JNS-Glx86TFBJLU-qu1VTnl9hPV6SHbbA01OCELSFlLR8aW3p3MSfLJIXL-Xitk0d0tf0cEb1o_Vs2eS-dwe4mB70e-kgPIFLdgnbweyCuGE6tFpJUSYDyx-ONO9iMSW4VH5S_0MR7pdLDI4_OqqtCkFOCgP8WCiQ8ph9ne-lioEZsPd0OlMxOVSf1IqW1pO0RikETAkGNzB7EsgtBIEjGt3Kz-o8Qg7AhI9kWd9rOVZUEOO--GJPgTKpBt_3r6BE-bRzUGJGrDM0zN_yt-T90bBBlgB7kVJElKpP6x5piI1OrxohINjwnEqvuER_8OxdInDrgPUra8YJo_FGbGyXwBr8Pex6Mel5W1fkZPqFk-b_zq-vdDh8tSOi1zJYo73EgrgV1cr-g7xgvebuxkCBfn3kHtW0JvfoCxkfBxl8o6F2itGCn51bFZA9cvYLZGUl1XurWOXMIZCxnlGEi31AtneG7HDbn9XHWlQoLcPDJElnQtZ734yFPTDTZFCAZkgLagoNeL2UwRBhSkGNDx-kRShCeBtLTwcdocWczijny31MMUamlo8dIs2f1UJ5X_8kglCkjrJLhsT4H4K6WltGnXUJG5JU9zOiAuRxc1ApZnUNvanvDGY2B9a6BW-TJ9uaBvHZNFvimXWE71yHQDvlbQPhhaGAB8bwx9zuTnJJYRwB1m1rjRAFE10vmtG961ulIe14qNspiT_YfOiJsMk3k5T6VvfvkhXFNCXiwGBiOqE-Ec0DCQp1wja5OyyGtw1CvukC87eSOXLuMsCzWhEXyvwxzQJvLttlBlTJxV8ogGa8Vpu_i2PsxEt_0Nk4ggOSl70yI9vP9vjP4cOo7wm8VnoRLPrE-ZNLZv_N-gE1I2xKmy39EaljAGezhDdQMaVYqx-S9GKZlo9MyVoPPTk5iujBXGgfXupC2aYFxgwRHDhUuoUiGEGpyMpYQrV9SUWvvekFIwdb-6KgudIasRW6Pq_O7w-JfyFr-CrGmT6Wvue0nP9yMyg.b9Am9NbHAqlJBphjszqBuA";
#endif

        public UserData()
        {
#if !UNITY_EDITOR
            user = "null";
            accessToken = "null";
            refreshToken =  "null";
#endif
        } 
    }
}