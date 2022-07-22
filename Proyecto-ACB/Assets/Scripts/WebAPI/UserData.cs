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
        public string user = "081ed05b-72d5-4e35-9ed3-d3106ac70758";
        [TextArea][SerializeField] [Tooltip("accessToken para la autenticacion del usuario")]
        public string accessToken= "eyJraWQiOiIwbWUrcFc0a3lEXC9VaGVlMDZ4RFwvTnM3UDZPNjVuZjZRb2tRakpFdTVVMXc9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiIwODFlZDA1Yi03MmQ1LTRlMzUtOWVkMy1kMzEwNmFjNzA3NTgiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMV9ZTk5QczhhWFYiLCJjbGllbnRfaWQiOiIzZzQzNHMwOGw2bDBibnU0dGl1NHRkdm9kMSIsIm9yaWdpbl9qdGkiOiJjZmNlMDc2MC1hZjBkLTQxYzctOTM1OS02Njc3NDViZjM5MTkiLCJldmVudF9pZCI6ImRmZGY0ODNiLTVkNjctNDE5Ni04MzI0LTk5MGI4MmE5ZTIyYyIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4iLCJhdXRoX3RpbWUiOjE2NTYwMTUwMTQsImV4cCI6MTY1NjAxODYxMywiaWF0IjoxNjU2MDE1MDE0LCJqdGkiOiIwZjVhNjQyYy05Y2UxLTQwMmEtOGNjMS1iZjNkOGJmMTZjNWIiLCJ1c2VybmFtZSI6IjA4MWVkMDViLTcyZDUtNGUzNS05ZWQzLWQzMTA2YWM3MDc1OCJ9.f2bcQeHn0xcZydzxa7wwohUfNQbzTvE3Fl4wvPNbprPGzreWfW56fW2_6bU9iqdesrKkZOFiSxdE55DzHQhqo77e19cVoiUehc7kihwQ1LMMx7oiHRwnIZsFRZsNZD_496P4GbUQEKeLzRzZ6aUsj2T3DGSJjx05pZ3TtCbhhFKfGl9OxFJvm_XZFf1go6CKtMPvBAXKXETBjbTVrTD4Uo-VLrbDSpCylvanuqtheQtmHd12YCHxDdil9d3I4iegcuhg-hc1oP0NPYvw0LCFDICKsJd10i6CHaQiIY7NwKlVTvBunMH3oVx-a5F6X9n5fdvd8FKomsVMIhRDeumlQQ";
        [TextArea]
        [SerializeField]
        [Tooltip("refreshToken para la autenticacion del usuario")]
        public string refreshToken = "eyJjdHkiOiJKV1QiLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiUlNBLU9BRVAifQ.M2XCLU_wLWIXomCUt2Z6TBtEMEMFaCtp7najcGU7k9_c45MnWTtBM4XGUydF1lq-wh49DJrjGt5Sj77wyXXQdXkqdL2mRZ5a2-DBgWNHb44xBnn1Pga2QxYRVrJR0XmRPyccF4nNZWvp6O_2__2YhEuM1JTuXlZdzuR6sRiBBk_3Z0jq5NrpVEbCiOCh0YIj5yejhe3JawycyrLPEgRZUYBqs7PO7MRZHmRidyY8S3qi-5-5RtRj3bd99jYOoIQy9uj3-Qv5mxmhLc3H_Zn-K2TuInOB41whxpPnrFEtcV2yoSHJBCa7qR2ZYJheSUnsS9RWosV-SxP49mrYSSxErA.e0juT6_7Ye5k1wHl.iD-Y3Z_S-0Ay_tgodL3cNx-7wFHsW6MsMmTvbYuIIYW07EIZbtxl7H_lbbHZJSIefPI57Uhlx_n5kwGGTALkL3NsKjGRRvaz2_bJ-ZgEAaySBWscTfFw3T13-UmFFIVnR-aCjMl9P5nm7ENY6CxXGDS478haL-DgrEYC4mNee3o7MIwi73pw7X47mUwTmPMeZnDZD3McnsmErdRreMDWRL6zn9KNWP3jV6XlnWtMIGev9BcqVqrIBfD8MO8WSwakjZMjzwECbcyiCS0vDf5hNfoMUVtpj0iv6GHKq7SyCGs80Ts10mz9M5RMl22RNmbzEUVJkzPfpXz5bQlg4X8is9mL1xg8f3iFtJ_b5eLSddefHi670rjANj7TgqkaHUh4o4t3TjbD5x9NZUAon_T-9Fkb7R2v88i4X3oE1lRJeEcX0MWp-cyvBnro8VU6s4Zgp5KfUAKQ-nOndrXOfVSY7sDx4rPcjPUCC3JqHJg09ed2Q31Qz1Oy8H_N11IHusXVUDzZ0XgwSGXHuFIjzRoIxJt5-EI8p8r8KpcWfA2iAExJvfWNzMeLcq7kvTjNFkjt22YvGuAf2RWtWTFcwq2XSwb3SK78WdkcGrktMmjLAwKUUJBTXJcB4747hNT8gZnoqF8PIzy9NbOfmhwc7yPi7kkkqjjbX5WCIQZgLu9qcmLcvCvLZ7x_smXYqWYpNFkJVTwkA8uSl_n2qUfxhxwXLC6yc2NiEZgZBCTkgETnSQ1pt8GSIodX70kdPcTsvDkraMWtQGZLQm1_-YC0yYueJZqlfT-ynHGS7ppKItdfpxNzDCtgYSijFw5bZbFPkynJ3r5VwxSmhSN5eLANcSL2WJtbTgJxiX8oeQJpJvHIWgdCxwuDw_OogHa6-_LsvQPOOvpm4A6jomAd7nn_AHiZZAQhqMINCi07wN-yNufhLZLmDDIm4-r5SXmRA4Cnubwc-bAA-92awkLaH6bCaeKidEYmmfwEK4wWc9QCNYVZSa2sdUFJNG7JFkqedB5IWgoKIe1TgFGWTtZR4awDjBrGQM1nRmii09lMRm9q5-lV3FmvxqMWDCo-7BGkc7KD-oSKk3D60T1TkbMot6Rk5Lzo2G9IoWuUn2GL86VjIi4o9oh4UBttDc1b59ptRRxujTrExHdQVrFsfbuuhx9qXuEWHzD5jXrpdmP1WrE7TusuzcleZlFeeLqWh1t-mlxuarJHkUyE1zmg6ZisqZ2JiCRmwnP9LEzC5zFw56NsPaKLutoUdbnmI3g2BnVZ0adINWeVl8u967ATZCObTdXd4Wa_gxU7GLHr6uYpa3ihlck8USiWOo9lURQO1A0rdkmNgg.PAMvPxMb7Gyqa8cnDiyEEA";
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