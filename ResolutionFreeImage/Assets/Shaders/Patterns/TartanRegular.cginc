#ifndef RFI_PATTERN_TARTAN_REGULAR_CGINC
#define RFI_PATTERN_TARTAN_REGULAR_CGINC

// ## Calculate EXACT distance as possible as you can in pattern functions. ##

#include "ShapeUtils.cginc"

// Simple tartan pattern sample
// linew = (line 0 width, line 1 width)
fixed4 tartanRegulatPattern(float2 uv, float es, float2 linew, fixed4 basecol, fixed4 linecol0, fixed4 linecol1) {
    float2 p = frac(uv);

    fixed4 col;
    float2 d;
    float t;

    d = abs(float2(0.5, 0.5) - p) - linew.x * 0.5;
    t = (edgeWeight(d.x, es) + edgeWeight(d.y, es)) * 0.5;
    col = lerp(linecol0, basecol, t);
    
    d = abs(float2(0.5, 0.5) - p) - linew.y * 0.5;
    // d = abs(abs(float2(0.5, 0.5) - p) - linew.y) - linew.y;
    t = (edgeWeight(d.x, es) + edgeWeight(d.y, es)) * 0.5;
    col = lerp(linecol1, col, t);

    return col;
}

#endif
