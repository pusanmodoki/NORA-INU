using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyA
{
	public bool isBoolA { get { return m_isBool; } }
	public bool isBoolB { get { return m_isBool; } set { m_isBool = value; } }
	public bool isBoolC { get { return m_isBool; } private set { m_isBool = value; } }
	public bool isBoolD { get; set; } = false;
	public bool isBoolE { get; private set; } = false;
	public bool isBoolF { get; protected set; } = false;

	public float valueFloat
	{
		get { return Mathf.Clamp01(m_floatA); }
		set { m_floatA = Mathf.Clamp01(value); m_floatB = m_floatA; }
	}

	[SerializeField]
	bool m_isBool = false;

	float m_floatA = 0.0f;
	float m_floatB = 0.0f;
}

public class PropertyB
{
	void Hoge()
	{
		PropertyA propertyA = default;

		bool isA = propertyA.isBoolA;
		//propertyA.isBoolA = isA;  //ムリ

		bool isB = propertyA.isBoolB;
		propertyA.isBoolB = isB;  //イケル

		bool isC = propertyA.isBoolC;
		//propertyA.isBoolC = isC;  //ムリ

		bool isD = propertyA.isBoolD;
		propertyA.isBoolD = isD;  //イケル

		bool isE = propertyA.isBoolE;
		//propertyA.isBoolE = isE;  //ムリ

		bool isF = propertyA.isBoolF;
		//propertyA.isBoolF = isF;  //ムリ
	}
}

public class PropertyD : PropertyA
{
	void Hoge()
	{
		PropertyA propertyA = default;

		bool isA = propertyA.isBoolA;
		//propertyA.isBoolA = isA;  //ムリ

		bool isB = propertyA.isBoolB;
		propertyA.isBoolB = isB;  //イケル

		bool isC = propertyA.isBoolC;
		//propertyA.isBoolC = isC;  //ムリ

		bool isD = propertyA.isBoolD;
		propertyA.isBoolD = isD;  //イケル

		bool isE = propertyA.isBoolE;
		//propertyA.isBoolE = isE;  //ムリ

		bool isF = propertyA.isBoolF;
		isBoolF = isF;  //イケル
	}
}