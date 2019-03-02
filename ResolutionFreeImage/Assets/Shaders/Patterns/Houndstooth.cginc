#ifndef RFI_PATTERN_HOUNDSTOOTH_CGINC
#define RFI_PATTERN_HOUNDSTOOTH_CGINC

// ## Calculate EXACT distance as possible as you can in pattern functions. ##

#include "ShapeUtils.cginc"

fixed4 houndstoothPattern(float2 uv, float es, fixed4 col0, fixed4 col1) {
    
    // (-1,1) = x2
    float2 p = (frac(uv) - 0.5) * 2.0;

    // Distance from diagonal (0,0),(1,1)
    float2 p1 = (p.x < p.y)? p.yx : p.xy;
    // const float ISQRT2 = 0.70710678;
    // float dn = dot(float2(1.0, -1.0), p1);
    // float dn00 = dn * ISQRT2;
    // float dn05 = (dn - 0.5) * ISQRT2;
    // float dn10 = (dn - 1.0) * ISQRT2;
    // float dn15 = (dn - 1.5) * ISQRT2;
    float4 dn = (dot(float2(1.0, -1.0), p1) - float4(0.0, 0.5, 1.0, 1.5)) * 0.70710678;
    
    // Shapes
    float d0 = max(p1.x, -p1.y) - 0.5;
    float d1 = max(abs(p1.y) - 0.5, dn.y);
    float d2 = max(max(-dn.y, dn.z), p1.x - 0.5);
    float d3 = max(-dn.z, -0.5 - p1.y);
    float d4 = max(-dn.w, p1.x - 0.5);
    float d5 = -(p1.x + 0.5);

    float d;
    // d = d0;
    // d = d1;
    // d = d2;
    // d = d3;
    // d = d4;
    d = min(d0, d1);
    d = min(d, d2);
    d = min(d, d3);
    d = min(d, d4);
    d = max(d, d5);

    // make x1
    d *= 0.5;

    // vec3 rgb = vec3(0.0);
    // rgb.xy = p;
    // rgb.xy = p0;
    // rgb = vec3(-d, d, exp(-d*d*10000.0));
    return lerp(col0, col1, edgeWeight(d, es));
}

#endif
