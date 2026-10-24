#!/bin/bash
# Navazující OCR Kunzeho (sken 946 stran). Přeskočí stránky, které už mají .txt. Použití: ./kunze_ocr_resume.sh /cesta/ke/Kunze.pdf [výstupní_složka]
PDF=$1; OUT=${2:-/tmp/kzo}; mkdir -p $OUT/img $OUT/txt; cd $OUT
for p in $(seq 1 946); do
  n=$(printf "%03d" $p)
  [ -s txt/p-$n.txt ] && continue
  pdftoppm -r 200 -gray -f $p -l $p -png "$PDF" img/p && f=$(ls img/p-*.png | head -1) && tesseract $f txt/p-$n -l eng --psm 1 >/dev/null 2>&1; rm -f img/p-*.png
done
cat $(ls txt/p-*.txt | sort -V) > kunze_ocr.txt && echo DONE
