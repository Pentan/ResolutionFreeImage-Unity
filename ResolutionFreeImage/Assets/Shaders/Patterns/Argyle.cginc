#ifndef RFI_PATTERN_ARGYLE_CGINC
#define RFI_PATTERN_ARGYLE_CGINC

// ## Calculate EXACT distance as possible as you can in pattern functions. ##

#include "ShapeUtils.cginc"

// asp: aspect ratio, lw: line width
fixed4 argylePattern(float2 uv, float es, float asp, float lw, fixed4 basecol0, fixed4 basecol1, fixed4 linecol) {
    float2 SIZE = float2(asp, 1.0);

    float2 p = fmod(uv, SIZE) * 2.0 - SIZE;
    p = abs(p);

    // Square
    float d0 = dot(normalize(SIZE.yx), (p - float2(SIZE.x, 0.0)));
    d0 *= 0.5;
    // Line
    float d1 = abs(dot(normalize(float2(-SIZE.y, SIZE.x)), p));
    d1 *= 0.5;

    fixed4 col;
    // rgb.xy = p;
    // rgb.xy = ap;
    // float d;
    // d = d0;
    // d = d1;
    // col = fixed4(-d, d, exp(-d*d*10000.0), 1.0);

    col = lerp(basecol0, basecol1, edgeWeight(d0, es));
    col = lerp(linecol, col, edgeWeight(d1 - lw, es));

    return col;
}

#endif
