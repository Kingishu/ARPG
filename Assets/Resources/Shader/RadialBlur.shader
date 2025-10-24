Shader "Hidden/Shader/RadialBlur"
{
	HLSLINCLUDE

#pragma target 4.5
#pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

		struct Attributes
	{
		uint vertexID : SV_VertexID;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float2 texcoord   : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	Varyings Vert(Attributes input)
	{
		Varyings output;
		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
		output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
		output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
		return output;
	}

	float _Intensity;//ǿ��
	float _SampleCount;//��������
	TEXTURE2D_X(_InputTexture);

	//����ģ������һ�ִ���������ʷ���״����ģ����Ч����
	//����ģ����Radial Blur�����Ը���������ܺõ��ٶȸ�

	//����ģ����ԭ��
	//ͼ��ѧ��ģ���Ĵ���ԭ����һ���ģ����Ǵ�ԭ������ΧȥѰ�Ҹ������ص���ɫ������ͬӰ�쵱ǰ��������ɫ�����˹ģ�����ǽ�ԭ�����������ص���ɫ��Ȩ�����Ϊԭ���ص���ɫ�Դﵽģ����Ŀ�ġ�
	//��ͬ��ģ������ȡ�ܱ����غͼ�Ȩ��͵��㷨��̫һ��������ģ�����ص��Ǵ�ĳ�����ĵ�����ɢ����ɢ���������Ҫ���������������ڵ�ǰ�����ص�����ĵ�������ϣ�ͨ���������Կ��Ʋ����������Ͳ��������ľ��롣���ص���ɫ���ɸ����صĵ������ĵ�֮�������Ͻ��в�����Ȼ������Щ��������ɫ�ļ�Ȩƽ�������ݾ���ģ�������ԣ���Ŀ���Խ��������Խ�ܼ�����֮��Ȼ��
	//��ˣ�ʵ�־���ģ���Ĵ����������£�
	//ȷ������ģ�������ĵ㣬һ��Ϊ���������ĵ㣬�����ĳ����������ĵ�����ĻͶӰ���ڵ�λ�á���ע�����ĵ���2ά���꣩
	//���㵱ǰ���غ����ĵ�ľ���������߶Ρ�
	//���߶�������в�������Ȩ��
	//��ģ���Ľ����ԭͼ����һ�����Ӻϳɣ�������Ҫ��*/

	float4 CustomPostProcess(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		half3 col = 0;
		//-0.5 ��ʾ�������ĳ���ǰ����(input.texcoord)������
		half2 symmetryUv = input.texcoord - 0.5;
		//��������ĳ���
		half distance = length(symmetryUv);

		//��Ҫ֪������ξ�������Զ����һ��
		//ǿ��/��������*����   ��˼��ָ:��ǰ���굽��Ļ���ĵ���ξ���*()
		half factor = _Intensity / _SampleCount * distance;
		//���ж�β���,������ɫ�����ۼ�
		for (int i = 0; i < _SampleCount; i++) {

			//ƫ�Ƽ���
			half uvOffset = 1 - factor * i;

			//symmetryUv:�����󵽵�:��Ļ����ǰ����ķ���
			//�÷���*ƫ��+��Ļ���ĵ�0.5,��õ���Ļ���ĵ㳯���������һ����ƫ����
			half2 uv = symmetryUv * uvOffset + 0.5;

			//��Ļ����= �����ƫ�Ƶ����� * ��Ļ��С
			uint2 positionSS = uv * _ScreenSize.xy;

			//��ɫ����(��Ļ���������,��������)
			col += LOAD_TEXTURE2D_X(_InputTexture, positionSS).rgb;
		}
		//���Բ�������,�͵õ�ƽ������ɫ ��ģ�����Ч��
		col /= _SampleCount;
		return float4(col, 1);
	}

		ENDHLSL

		SubShader
	{
		Pass
		{
			Name "RadialBlur"

			ZWrite Off
			ZTest Always
			Blend Off
			Cull Off

			HLSLPROGRAM
				#pragma fragment CustomPostProcess
				#pragma vertex Vert
			ENDHLSL
		}
	}
	Fallback Off
}