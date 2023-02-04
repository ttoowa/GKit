using System;
using System.Text;
using UnityEngine;

namespace GKitForUnity.Unity.Utility;

public static class TextUtility {
    public static void SetMultilineText(this TextMesh textMesh, string text, float maxWidth) {
        textMesh.text = text;

        if (string.IsNullOrEmpty(text)) {
            return;
        }

        StringBuilder builder = new();
        CharacterInfo charInfo = new();
        char[] textArr = text.ToCharArray();
        float widthStack = 0f;
        for (int i = 0; i < textArr.Length; ++i) {
            char character = textArr[i];
            if (character == '\n') {
                widthStack = 0f;
            } else {
                textMesh.font.GetCharacterInfo(character, out charInfo, textMesh.fontSize, textMesh.fontStyle);

                float advance = charInfo.advance * textMesh.characterSize * 0.1f;
                widthStack += advance;

                if (widthStack >= maxWidth) {
                    widthStack = advance;
                    builder.Append(Environment.NewLine);
                }
            }

            builder.Append(textArr[i]);
        }

        textMesh.text = builder.ToString();
    }

    public static float GetAdvance(this char character, TextMesh refTextMesh) {
        CharacterInfo charInfo = new();
        refTextMesh.font.GetCharacterInfo(character, out charInfo, refTextMesh.fontSize, refTextMesh.fontStyle);
        float advance = charInfo.advance * refTextMesh.characterSize * 0.1f;
        return advance;
    }
}