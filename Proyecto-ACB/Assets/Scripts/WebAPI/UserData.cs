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
        public string user = "b504a40c-eebc-401e-936a-c1a90673d257";
        [TextArea]
        [SerializeField]
        [Tooltip("accessToken para la autenticacion del usuario")]
        public string accessToken = "eyJraWQiOiIwbWUrcFc0a3lEXC9VaGVlMDZ4RFwvTnM3UDZPNjVuZjZRb2tRakpFdTVVMXc9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJiNTA0YTQwYy1lZWJjLTQwMWUtOTM2YS1jMWE5MDY3M2QyNTciLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMV9ZTk5QczhhWFYiLCJjbGllbnRfaWQiOiIzZzQzNHMwOGw2bDBibnU0dGl1NHRkdm9kMSIsIm9yaWdpbl9qdGkiOiI3YTYzOGU2Yy03MzE5LTQyYjAtYjRkNi0zNTE4MTNlMjk4YTkiLCJldmVudF9pZCI6IjNlMGMyZTEzLTRlOWYtNDFhYS04NDJmLTBhMDA1MjdhMTRmZCIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4iLCJhdXRoX3RpbWUiOjE2NjkyMTE3NDEsImV4cCI6MTY2OTIxNTM0MSwiaWF0IjoxNjY5MjExNzQxLCJqdGkiOiIwMGNmNjMzNi0yNjQxLTRkY2EtYjA2Yi04ZjlhOWM4MTMwOWUiLCJ1c2VybmFtZSI6ImI1MDRhNDBjLWVlYmMtNDAxZS05MzZhLWMxYTkwNjczZDI1NyJ9.Zfsz2k00-g9LwjwU1OEkwZlp9SkjEv0UOIY62kL1TuKO-rLfvRYqtWCmTR-asyfrkZSLkJdjfmYviOQh2kfhSye6Dld6mMMQTqi9Twq0x3c5InPGfS4PVTiY_4-RApuYiDiiRDzV0_0Fp_mRdmn9tcaeH-DlV3T1P4qhFXezWCkiDLXyAN1vk1_IKsG9rMHpxyOz6CHhn0BXIuz1MEpiPVtzEChdwugG0J_z3nT1AMuUIrm6CxXyGk2g1weDd8rQpWjmkYekq3YsCfmkhv2nSpOVGC9xmlgnitQCb19L0zSvhnGEN75ghwfKeVLSfpPN8El6wXMFfBx8LJibZs62bg";
        [TextArea]
        [SerializeField]
        [Tooltip("refreshToken para la autenticacion del usuario")]
        public string refreshToken = "eyJjdHkiOiJKV1QiLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiUlNBLU9BRVAifQ.HmwYRAj-ke_lRDt8phN5Q8TnX5Ix-jpbAp6Lts7TcPh74VsaFJT6-fBTjhvmMSKWh8uYG1eOqXCuUxKSLpoQ1iuBW679kAA74D-BCgr7rz0T4BECV2rOLzh1zNlo1W_Y40CcVMJKBR6534c01Q2lXt2LWmW5ICccxl9m4iwTHjCsw4H9ATNMNRaZaiFkjVravlYVkEJIGYztl5HLuuMJYtZgtgY34ANhDsFigmQUS8AxPwG4fDmPiBlDPRzndIy7qGGTY3Nz6Xx5x-YJRJii2Gf4gGTVmjIKQ5b_zuqyPcs_gVvP80wJiQrW10NbLxX9BBbD7UT82SMrgjc7mT-r2Q.6g3J3jWNh2Xm12w5.yuKgWEN6pALUE4G0CBQGPwtnuZ1jUD14Yl_0bYBne1P9ZTr7iM3-_ETfYbLvuJ_VAHH-cSttio523rADePmF_bdtfw7s7IjQLveG-hPjPBTrWQFnWVyzRsUh6tbBmPo7F1rY9rj-zqYWfi3WnGUnuwxkOcpvgtGClaWf6RB_LHGllVgSSxbTgicwuLXp4wQIj6EOVjrDj9V9MYudk9SqIB2fmSb0jL-DJuo_rbqnX36PyJPwxfefOabo4pll1hKSnDORnb4lO-p2KqZFPcI9dUsD0XW2xBITmNp8Ne6XMymr0GLRL5V-qnnXb-e_Ue60L4OV81Z39b-tZZ_7zFV0ZZwkrRSoi2K70k92-G8ff1ZzfFoGWLmjZziBdxgC6R6dTiI0kl_YW2VjCgNJtdezxuMNjo-0JXd1b32C6USZMxAIhMTsoXj9UCyisSAWExyoc4Z_hhXVGoqvx-GztHHl4EkGPKQS-OlDMtWhVvWLNMrnFmtVpWx9RStv70xkTHmXBddla0TDPkOhgqQIsIY6ZV31vmVFlv-D3t7VllOICU0mF1IYmAtzOmc0x2hKRmjGuujgzotTY8kc7eP4mIZ_yRCeB8D0R7yHUnqcq7e3nBtsOHor-zRYmY5k6s90NLVWiyUGygaG1fJcfpQfDU8DlrMiI2YiRdMPGktn7ENcICecosyaBdv9P2HuzVzNcwsMbxofebFOe0ceH0n9XZBAxjMdYbUNq6IiALlCwJjEgH2L67aQWMyrCQd4abFxTn0x4jT5HPBLaCNa1b2jqQCaPuZ08uXbDCIIC9-UoUgkwucbTMnT9e_a8MFMIe1eCOIhiWtabaDVNHeY7YbnfaBlwVIK6hXlOMiifzHGmZ-gLn0Jz8wN3BZ2_EBGQgVufnbkYwnBw6xq2c4HTWy7HFZTrIyNwzwvxulx8469EmaaCQe6Sb6BEC8RxluoT3G3yWhk0rDlAXrN014ZKw22lhL1V7z7uv_8_ZkmIs48tGgdifXaiYR1EbtWsfr0OmrcUiW8nMutD1QKCodt2imzpVJclJW-mXSpeh9wH8YXyTvY9YWh6pIpVmQYImpaJdWd7O3Spy_r6l2PJ7VtUITVIt_tHAys8gI0HuvVkQbQUq6m7WjnuuL1jquAhF6htfGvft_G0E4DfHpqTG0jY0MWt4-0ZMlCEXTFxYLKXJL9pg1aYEXCXl1Hdx6AdM_sZvDR7uuDKQ6S1-enFkBGBZa6mZ456gnNr25S3edBPLpaqMvrhQsAGYALntiUX6qsqZ03FVFrzbT7f_Yn_cFtWIwIgKyOaAeKCr6W7oDJRIVPxYTSdoQkMEa8U3prZk6Bj4K4Pg.0lzKEa_A0gO6jx12IGek3g";
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