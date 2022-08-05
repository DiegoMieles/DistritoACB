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
        public string user = "191c83c9-b95f-40df-b285-3f202e1aebf2";
        [TextArea][SerializeField] [Tooltip("accessToken para la autenticacion del usuario")]
        public string accessToken= "eyJraWQiOiJtc2pNc2NYdk0rWjA4UXZVa3VTRFZiWGRFYmZBd0lRSWVqeEZtOGE1NEdnPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiIxOTFjODNjOS1iOTVmLTQwZGYtYjI4NS0zZjIwMmUxYWViZjIiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMV9GWm1aRnpKR0wiLCJjbGllbnRfaWQiOiIzaDNkN25qZ3F1aHJqNWhqaTB0b25rZG5rayIsIm9yaWdpbl9qdGkiOiIxZWNkNzc5NS0zZDI2LTQ5ZmEtOGRiNS0yNzQyODQwZTVjODEiLCJldmVudF9pZCI6ImFlYzFmMmJmLTBhYTAtNGM5OC04ZWQwLTQ0OTZjOWYyODA5ZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4iLCJhdXRoX3RpbWUiOjE2NTk3Mjk4OTksImV4cCI6MTY1OTczMzQ5OSwiaWF0IjoxNjU5NzI5ODk5LCJqdGkiOiJhZDU3ZTlhMC01ZmVlLTQyYWUtYmI0Ni1lYzY2MmZmZTFiM2UiLCJ1c2VybmFtZSI6IjE5MWM4M2M5LWI5NWYtNDBkZi1iMjg1LTNmMjAyZTFhZWJmMiJ9.GH-mX4nMzsP8iAC8TWWRffBJLjBgaJlGXooyHbmPVCU5RLvshDtkxGGm7Zp6MauWu8_lq-gEtqTlWTAko7H0AN0QUtLogPUlOPvO2i80HcNkd8xCBz0zjtlZz8f-HLVUZwymq3sTohVh58VgALRpQwfdMXc7_kTHbZ_nIW2p64SG02tThSGfC3WAQhvS30UW2eMuD00s3rwrqLOEN345KTlzoqGJJVfONaZYsIHHWUMdk8D549FyawN6GEHmEzjRby6xlM7yFfWAS53pPkPW7Md9DKsqJ64gj2QoV57lgEu5Y5KZoaKpn3i3tBk1A7N7DEu_LusAcYZpW26qN_A-2w";
        [TextArea]
        [SerializeField]
        [Tooltip("refreshToken para la autenticacion del usuario")]
        public string refreshToken = "eyJjdHkiOiJKV1QiLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiUlNBLU9BRVAifQ.DQo6DPjmz51Hy0UZKPd1aXp3xWxOgg_A-pl4pUhCebRUDxo5rqrpMapUDHC9WUouBHJ7k2rhar51Mo91xZS4nxfkECMSLnr14Zhlagb60xJuhxP-yVY0rdd3P3XikX8A7ORQC_Vmib1wCzjb9aOLKQKBytWb8N48xqjJJPwlL3OwVbjogJ0oDIcN7HPSbsAoqxNDflLO0gnS3a2RZO5rY5Li0rk_1UXcodeh0FcUlu40PRHOiLUrDwfky3oN0zmuqCdUCnmmkrevA8Qimm5tBH-YyR4BpZy6NhEJJ2YPXdtDLayh-v1UVp5_TWztDnjpaj3EdPtHpeCRcZHXrza3g.ADSyfP81hIVDSsTA.iJkZMd-apfo0aakPQZvcrZKoB83S2YRxZGdozFfwLFzG0e_BhTMI8vasoHWbJIbqsElYEhzjcCJ3ZpYf0w0DCspyaNiUBdiasVTagzEgMVvZojxTmO53VsUMOtzj0XCMrn9DFfIYUEJAlvkwP_HEfFqKU-fOpOZZGET0ggNpos9fORa4v9v_WfNajIqBfLasSVpU_eSPEAR0cOGDFOlVJgBW6Cy-gOh6r8LdqFANwMfXGtwnOiuM3zLazuJHTpt-p5skUUzj944M2gYXTK9lk6ufhUpy0J4szN6ISPRPAJznX73M7NhfOmAQmiOWIJsj_BtFgXJZvX3mM0dtnsjYvJ3_rF3Czp4DqU45rDJ64iUhcW1C_5IP4e0WbGtyoM0A7vugh2AEknBwSAQ1YkrfDpqpwLkcsHUMGLB-vbBuT2KW1THGYNy3STaYeYZoQiUoLjPn6xTTQmWGC2txB_0uwsR--HrbRlRyAs1tpz6pOgSwxJawoEbgDi_CAN0aIShMDJNEo6cXlW7Cb4zmkVGXGtblJQAx0w68MgL3Bgf4U0wdaRm0sOqePQCJ8VVEwNkj3ipaCXYnGJqVdm52A4CJUrSm2JY8bGl2ZYQ5wYXKSyvAG3CmzZpdGYoCPTH-gE2HTF0i5bV7um1YHQQwlKuyWXDNzCxiu0_DWv5u76ZQ349Nmskbhi5nRKxPEHtEN9d929Kne7HdIhy7XJUiZjJmefq8-Sm4dHg0s3C2HBUN9-gN28nIEFdeBkUv6-LyPtxlcR6mEd6DRAfs5d78reKWLGPNRkdS3h_LfDV7UedKGyqHZblzVMkoCo8sysy0xTYS9TO0OYOBrS3SSiQiUbTb4gl0jkiZ80FUk_hmzBgA0kgClwUPBQhMLUUo3jAq_LJwM6uEXGb8n4EvKACJ1n61XJJ82eKc7R3srF1lzuBxd3DWXzqtEcwxdG_V6vZKOFqXzbs1W5SzRXdcX7XyiPHzQJuewpwJUJrCsw1HeAwDgZI_mCQhDdlVAU5BrPoTdCAopuTYLCFioxzY3SCN1louZYIk8Pat9sbdW6VWn6wuVxWmjB4kyVunFfccUdM5b0iPOqDElkp3TbS4QEwDnkLKj5o2FMghnVGHY7Y7tykVf7MD6FxUHL_Ifv8xgUcyunxKKsBBCFOKxVeOhAhg62fnxtaRLzopi7ESADqDXwfijrFRkrDb3CjdCfEkGTl3nkeCEcCwKHAlo9Bgkq_F9ZuqGyzvrU2Uk7eh1vDkd_qE6BFwA4MSVHoWXKuNiw8zF-UBqePuduXODMkRn714IFJ6bEv42TnjnW9VQpYxt27WPSKhSEi5c2528h_fQ.PGcQfOpEsMFhDi9PftSmuw";
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