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
        [TextArea][SerializeField] [Tooltip("accessToken para la autenticacion del usuario")]
        public string accessToken= "eyJraWQiOiIwbWUrcFc0a3lEXC9VaGVlMDZ4RFwvTnM3UDZPNjVuZjZRb2tRakpFdTVVMXc9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJiNTA0YTQwYy1lZWJjLTQwMWUtOTM2YS1jMWE5MDY3M2QyNTciLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMV9ZTk5QczhhWFYiLCJjbGllbnRfaWQiOiIzZzQzNHMwOGw2bDBibnU0dGl1NHRkdm9kMSIsIm9yaWdpbl9qdGkiOiIwNjk2MWZhYS05NDQyLTQ2ZGQtOGJlMi1lNzRhOTJkYzc5ZTYiLCJldmVudF9pZCI6IjE0ZGFiYjU3LWI1M2YtNGYyMS05ZjMyLTI5NzY1ZGQ2YTdlNiIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4iLCJhdXRoX3RpbWUiOjE2NjUwOTg0NTgsImV4cCI6MTY2NTEwMjA1NywiaWF0IjoxNjY1MDk4NDU4LCJqdGkiOiI1ZTJhMDEyYy1hYjE1LTQ0NmUtOWY4MS0zYzdkMzkwNmM4MDYiLCJ1c2VybmFtZSI6ImI1MDRhNDBjLWVlYmMtNDAxZS05MzZhLWMxYTkwNjczZDI1NyJ9.iy44E6RKACS_0_8qrql8QXFpKsDWh3zPv_xxg92v6G_fmuFQgI-KyrPIrT3NsQgmomgz6g_K9GbFS59RZpahCAQ71ZCVYKZABuAthdZUnuOdrIoRr8QkGWVq6daNkf76RHx8IedTQ-rfyzE8NfkjbOaKd76i-ibq94gEa0fsQCJEEAFmAW7PVx_asYtC0Y5YxLlHYWXwHIgG18NPPVFuzXXxKtIiU9UQhQFZJAVkyJCOf7__dz8LaD3oYaubufrimXr5vw3XmL6lNtoxLHD8ZtISs_oXRlyUWrJCTohpmwdiWfZElGgYETqQps01Sv6G9gQ32wxIZyuFZ5EwJQP_sw";
        [TextArea]
        [SerializeField]
        [Tooltip("refreshToken para la autenticacion del usuario")]
        public string refreshToken = "eyJjdHkiOiJKV1QiLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiUlNBLU9BRVAifQ.sGplXJYQKSn3TxRs954Z51AkP5X48EUW_SvjxuQm6Gds_hzmXnjLTr-kv8GE990TDKTOTbwJjN8Sj6uPXOgP0EPna9hzlezcu48tIptcDn75Jm9Z1Zwh8rXsGFlr74j00OICmIxJo8oDDvpeqcHTFkGEaPPJ3bmJn7quTq2NSpz-OMW4yyLSwO7VZq8XKnI-prTGAWinwoTEuS8ZzXQJ_Vo_QnWgB8Bw-o0OYKB1FNiSfmvJKrBQUu-Ci4s-gbTypEFuunBsYjtVGjrBUvZ4n4U57Tpe98PxygMIdFe7xjrBn8OgWvcRiTlg-nopSuxI6nD4oE1VZV62eAF0Vxe4aw.p8E4JLzShDNpX6Ir.vy9xWuASKdN-S5s7pimv83fT3lDMVX1j-bQWha-DkI3DvnGCIdhyLKDPfwrSVtXDW08HMEVajIp8J4EmP9HSEs3r-S-coS8FYD-LLVBp0vs7N-WApWmde8v0m2NPeMhSLCOOT8NqBRFl7fLdZ6RfNEkXmQfs8wCWvNES0B7SqU4pORzKyIRxtpIYi1KQXODBwcJD9KnH79oA52vaCzn7wMtwZMSqahy_cwHTlOh3qrSTeyzLWPWXLqqZLjDGqyOEJLf4FiR_0UI-idJniwfFB_0KqWVLv3mIaxUrqrg5r6YQpuQyYKMnfwxoivltR4uL4IiakQ-vtpu4hwPcqBi3d7cEaC_aCWzKTxFsJZGoqQWNSg1MIvTXvuv2uHcSu7LufgptQD1sNxx7U5pW0igik6nvLGinHoTPgah9iulA2rjh114TQz7hHSP8hbgC3FgRwW86Kgow3wqQUXN7LiAlMap-m6rDLxrJWeyY5GZ3gVrO7ZeHiRyUyyRcxCapPqneJ3IIHe_Mtf2ddGXHRzsXckSlDbzRFjSTCCdIshW27V11l6WN98rIP7VMvJaj7xoX30Dp6qP8LOa8JHcPAk0q8UJ5gSLkztNdNE9pgtZnviIbJMCIynTtTqLy_WAZmRfnGw3FKP3IkyRpw6oBvH2x0Rr74hWETOwhXc0BC1IAkrP18qQz-oQLnfEHb1kh64-tFjZnH8KwfJhU6KE-inPtuNwbAuRLknVwuKEJkyXEvHKk-vwadzp2p1T8I_U0llRNWD2IJyuqY0PLlz2yq5pbTHzNngG0h-He9UUb0j5A6t1Jyb5giHVxirXUG6XrJCXoAMp2SxDuptglgmdy4BZE86E7fPqb_lfqZ7zzlXNBLqqg20ae_5MqcbEe30T1y7Qg-1J2A-FjF1AjaQcmapgeH8Blfox1qB6LGoiNtqu07mHNwR9AXMIdkweFqQFrzoWD26NI1pv1if-BoSxjupgf50fh07VTQ6NdKK0k-8JUGvkzqe0w2s5FB31SyBYLP873hn6Nlo9hUzdVYFSQ4SaZyLfR5mkbnAl1l-iAMMefyPPYxOXDoEDyp2E5ois4T_i9rYHqkGczDEpqNyuLI71WUbHsTciWKeHmCUaMPeVX5wEhDVFQS5GlQqPWbm9JQSGs5kGPkz7FYrqVmcDWC76ggpzKOaojTgysj0RBDXpysKXuyy5q-Xe6ZEVUDKyAMnWWl1ycznAtJY3QF1FycOjeISSLLRvOmlyTB9S9hlJGBHDg3wBWCi7jupC5nfVkV4nATpicVWzbsM5nwfhdoC9jy_Nm4ugHGZO4-Jbe8AuY9qkzFcY6hpjYR_Brk0tbZQ.PU0J9eyUc_sWTxlf6qCwrg";
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