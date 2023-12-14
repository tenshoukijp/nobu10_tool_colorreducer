/*
ダブルクリックするだけで一括で *.png を *.bmp に変換できるよう、
主にこのファイルを付け加えています。 by 翔.jp
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace iZYINS;

partial class ColorReducer
{
    static int showLicenseCnt = 0;

    static Bitmap reduceColor(Bitmap lSrcBMP)
    {
        TiZYINSoption op = new TiZYINSoption();
        op.palletnum = 256;
        op.dithermode = 2;
        op.ditherlevel = 8;
        op.wblevel = 10;//unused
        op.yblevel = 10;
        op.crlevel = 10;
        op.gmlevel = 10;
        op.outlevel = 8;
        op.inlevel = 3;
        op.addpalettealpha = -1;
        op.BenchMark = false;

        uint[] lFixedPalette = null;
        uint[] lAddPalette = null;
        if (lFixedPalette != null)
        {
            op.addfix256num = lFixedPalette.Length;
            for (int j = 0; j < lFixedPalette.Length; j++)
            {
                op.addfix256[j] = lFixedPalette[j];
            }
        }
        else if (lAddPalette != null)
        {
            op.addfix256num = lAddPalette.Length;
            for (int j = 0; j < lAddPalette.Length; j++)
            {
                op.addfix256[j] = lAddPalette[j];
            }
        }
        else
        {
            op.addfix256 = null;
            op.addfix256num = 0;
        }

        Bitmap lDestBMP;

        CWrapper wrap = new CWrapper();
        byte[] extra = null;
        wrap.gdwrap(lSrcBMP, out lDestBMP, ref op, writeProgress, extra);

        if (lDestBMP != null)
        {
            return lDestBMP;
        }

        return null;

    }

    static void Main(String[] args)
    {
        // 引数が存在しない場合は、カレントディレクトリの.png を 256色に「拡散誤差」で減色し、.bmp として保存します。上書き保存します
        if (args.Length == 0)
        {
            // カレントディレクトリを取得します
            string currentDirectory = Directory.GetCurrentDirectory();

            string pallatteFile = "";
            if (System.IO.File.Exists("palette.bmp"))
            {
                pallatteFile = "palette.bmp";
            }
            else
            {
                // ChatGPTに聞いたパレットの綴りミスの代表9例。
                string[] missPalFileNames = { "palatte.bmp", "pallete.bmp", "pallette.bmp", "Palett", "Palete", "Pallet", "Palet", "Plette", "Platette", "Palletee" };
                foreach (var missName in missPalFileNames)
                {
                    if (System.IO.File.Exists(missName))
                    {
                        pallatteFile = missName;
                        MessageBox.Show($"{missName} ファイルが見つかりました。\nファイル名の綴りが間違っています!!\n正しく palette.bmp というファイル名に命名してください。\n\n今回はこのファイルをパレットファイルと見なし変換します。");
                        break;
                    }

                }
            }

            // カレントディレクトリ内の*.pngファイルを検索します
            string[] pngFiles = Directory.GetFiles(currentDirectory, "*.png");

            // 所得したファイルの一覧をコンソールに出力します
            foreach (string srcPngFile in pngFiles)
            {
                String dstBmpFile = Path.GetFileNameWithoutExtension(srcPngFile) + ".bmp";
                // -D2は拡散誤差 -R1は上書き
                String[] newarg = { srcPngFile, dstBmpFile, "-R1" };
                if (String.IsNullOrEmpty(pallatteFile) == false)
                {
                    newarg = new String[] { srcPngFile, dstBmpFile, "-R1", "-F" + pallatteFile };
                }
                SubMain(newarg);
            }
        }

        // 引数が存在するならば、元来のiZYINSのまま
        else
        {
            SubMain(args);
        }
    }
}
