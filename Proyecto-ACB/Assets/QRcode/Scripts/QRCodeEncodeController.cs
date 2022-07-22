﻿/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>
/// 


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using ZXing.Common;
using System.Text.RegularExpressions;
using System;

public class QRCodeEncodeController : MonoBehaviour {

    [Serializable]
    public class UnityEventTexture2D : UnityEvent<Texture2D> { };

    public enum CodeMode
	{
		QR_CODE,
        CODE_39,
        CODE_128,
        EAN_8,
        EAN_13,
        //DATA_MATRIX,
        NONE
	}

	private Texture2D m_EncodedTex;
	public int e_QRCodeWidth = 512;
	public int e_QRCodeHeight = 512;

	BitMatrix byteMatrix;
	public CodeMode eCodeFormat = CodeMode.QR_CODE;
	public Texture2D e_QRLogoTex;
	Texture2D tempLogoTex = null;
	public float e_EmbedLogoRatio = 0.2f;

    public UnityEventTexture2D onQREncodeFinished;

    void Start ()
	{
		int targetWidth = Mathf.Min(e_QRCodeWidth,e_QRCodeHeight);
		targetWidth = Mathf.Clamp (targetWidth, 128, 1024);
		e_QRCodeWidth = e_QRCodeHeight = targetWidth;
	}

	void Update ()
	{

	}

	/// <summary>
	/// Encode the specified string .
	/// </summary>
	/// <param name="valueStr"> content string.</param>
	public int Encode(string valueStr)
	{
	//	var writer = new QRCodeWriter();
		var writer = new MultiFormatWriter();
		Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();  
		//set the code type
		hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
		hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);

		switch (eCodeFormat) {
		case CodeMode.QR_CODE:
			byteMatrix = writer.encode( valueStr, BarcodeFormat.QR_CODE, e_QRCodeWidth, e_QRCodeHeight,hints); 
			break;
		case CodeMode.EAN_13:
			if ((valueStr.Length == 12 || valueStr.Length == 13) && bAllDigit(valueStr)) {
				if (valueStr.Length == 13)  {
					valueStr = valueStr.Substring (0, 12);
				}
				byteMatrix = writer.encode( valueStr, BarcodeFormat.EAN_13, e_QRCodeWidth, e_QRCodeWidth/2,hints); 
			} else {
				
				return -13;
			}
			break;
		case CodeMode.EAN_8:
			if ((valueStr.Length == 7 || valueStr.Length == 8) && bAllDigit(valueStr)) {
				if (valueStr.Length == 8)  {
					valueStr = valueStr.Substring (0, 7);
				}
				byteMatrix = writer.encode( valueStr, BarcodeFormat.EAN_8, e_QRCodeWidth, e_QRCodeWidth/2,hints); 
			} else {
				return -8;
			}
			break;
		case CodeMode.CODE_128:
			if (IsNumAndEnCh(valueStr) && valueStr.Length <= 80) {
				byteMatrix = writer.encode( valueStr, BarcodeFormat.CODE_128, e_QRCodeWidth, e_QRCodeWidth/2,hints); 
			} else {
				return -128;
			}
			break;
		case CodeMode.CODE_39:
                if (bAllDigit(valueStr))
                {
                    byteMatrix = writer.encode(valueStr, BarcodeFormat.CODE_39, e_QRCodeWidth, e_QRCodeHeight / 2, hints);
                }
                else
                {
                    return -39;
                }
             
                break;
			
		case CodeMode.NONE:
			return -1;
		}

		if (m_EncodedTex != null) {
			Destroy (m_EncodedTex);
			m_EncodedTex = null;
		}
		m_EncodedTex = new Texture2D(byteMatrix.Width,  byteMatrix.Height);
	
		for (int i =0; i!= m_EncodedTex.width; i++) {
			for(int j = 0;j!= m_EncodedTex.height;j++)
			{
				if(byteMatrix[i,j])
				{
					m_EncodedTex.SetPixel(i,j,Color.black);
				}
				else
				{
					m_EncodedTex.SetPixel(i,j,Color.white);
				}
			}
		}

		///rotation the image 
		Color32[] pixels = m_EncodedTex.GetPixels32();
		//pixels = RotateMatrixByClockwise(pixels, m_EncodedTex.width);
		m_EncodedTex.SetPixels32(pixels); 

		m_EncodedTex.Apply ();
        
		if (eCodeFormat == CodeMode.QR_CODE) {
			AddLogoToQRCode ();
		}

		onQREncodeFinished.Invoke(m_EncodedTex);
		return 0;
	}

	/// <summary>
	/// Rotates the matrix.Clockwise
	/// </summary>
	/// <returns>The matrix.</returns>
	/// <param name="matrix">Matrix.</param>
	/// <param name="n">N.</param>
	static Color32[] RotateMatrixByClockwise(Color32[] matrix, int n) {
		Color32[] ret = new Color32[n * n];
		for (int i = 0; i < n; ++i) {
			for (int j = 0; j < n; ++j) {
				ret[i*n + j] = matrix[(n - i - 1) * n + j];
			}
		}
		return ret;
	}

	/// <summary>
	/// anticlockwise
	/// </summary>
	/// <returns>The matrix.</returns>
	/// <param name="matrix">Matrix.</param>
	/// <param name="n">N.</param>
	static Color32[] RotateMatrixByAnticlockwise(Color32[] matrix, int n) {
		Color32[] ret = new Color32[n * n];
		
		for (int i = 0; i < n; ++i) {
			for (int j = 0; j < n; ++j) {
				ret[i*n + j] = matrix[(n - j - 1) * n + i];
			}
		}
		return ret;
	}


	bool isContainDigit(string str)
	{
		for (int i = 0; i != str.Length; i++) {
			if (str [i] >= '0' && str [i] <= '9') {
				return true;
			}
		}
		return false;
	}

     bool IsNumAndEnCh(string input)
    {
        string pattern = @"^[A-Za-z0-9-_!@# |+/*]+$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(input);
    }


    bool isContainChar(string str)
	{
		for (int i = 0; i != str.Length; i++) {
			if (str [i] >= 'a' && str [i] <= 'z') {
				return true;
			}
		}
		return false;
	}

	bool bAllDigit(string str)
	{
		for (int i = 0; i != str.Length; i++) {
			if (str [i] >= '0' && str [i] <= '9') {
			} else {
				return false;
			}
		}
		return true;
	}

	public void AddLogoToQRCode()
	{
		if (e_QRLogoTex != null) {
			int maxLength = Mathf.Max (e_QRLogoTex.width, e_QRLogoTex.height);
			if (maxLength > (m_EncodedTex.width * e_EmbedLogoRatio)) {

				if (tempLogoTex == null) {
					tempLogoTex = new Texture2D (e_QRLogoTex.width, e_QRLogoTex.height, TextureFormat.RGBA32, true);
					tempLogoTex.SetPixels (e_QRLogoTex.GetPixels ());
					tempLogoTex.Apply ();
				}

				float scaleRatio = m_EncodedTex.width * e_EmbedLogoRatio / maxLength * 1.0f;
				int newLogoWidth = (int)(e_QRLogoTex.width * scaleRatio);
				int newLogoHeight = (int)(e_QRLogoTex.height * scaleRatio);
				TextureScale.Bilinear (tempLogoTex, newLogoWidth, newLogoHeight);
			} else {
				if (tempLogoTex == null) {
					tempLogoTex = new Texture2D (e_QRLogoTex.width, e_QRLogoTex.height, TextureFormat.RGBA32,true);
					tempLogoTex.SetPixels (e_QRLogoTex.GetPixels());
					tempLogoTex.Apply ();
				}
			}

		}
		else
		{
			return;
		}

		int startX = (m_EncodedTex.width - tempLogoTex.width)/2;
		int startY =  (m_EncodedTex.height -  tempLogoTex.height)/2;

		for (int x = startX; x < tempLogoTex.width + startX; x++) {
			for (int y = startY; y < tempLogoTex.height + startY; y++) {
				Color bgColor = m_EncodedTex.GetPixel (x, y);
				Color wmColor = tempLogoTex.GetPixel (x - startX, y - startY);
				Color finalColor = Color.Lerp (bgColor, wmColor, wmColor.a / 1.0f);
				m_EncodedTex.SetPixel (x, y, finalColor);
			}
		}

		Destroy (tempLogoTex);
		tempLogoTex = null;

		m_EncodedTex.Apply ();
	}
}
