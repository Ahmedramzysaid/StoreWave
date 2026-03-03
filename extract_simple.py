import sys, os
sys.stdout.reconfigure(encoding='utf-8')

# Simple extraction without pip install - use zipfile and xml
import zipfile
import xml.etree.ElementTree as ET

docx_path = r'c:\Users\Ahmed Ramzy\OneDrive - Faculty of Computers & Artificial Intelligence\Desktop\project MVC\LL(Final Documentation).docx'

with zipfile.ZipFile(docx_path, 'r') as z:
    with z.open('word/document.xml') as f:
        tree = ET.parse(f)

root = tree.getroot()
ns = {'w': 'http://schemas.openxmlformats.org/wordprocessingml/2006/main'}

paragraphs = root.findall('.//w:p', ns)
output_lines = []

for p in paragraphs:
    # Get style
    style_el = p.find('.//w:pPr/w:pStyle', ns)
    style = style_el.get('{http://schemas.openxmlformats.org/wordprocessingml/2006/main}val') if style_el is not None else 'Normal'
    
    # Get text
    texts = []
    for r in p.findall('.//w:r', ns):
        for t in r.findall('.//w:t', ns):
            if t.text:
                texts.append(t.text)
    
    full_text = ''.join(texts).strip()
    if full_text:
        output_lines.append(f'[{style}] {full_text}')

# Write output
out_path = r'c:\Users\Ahmed Ramzy\OneDrive - Faculty of Computers & Artificial Intelligence\Desktop\project MVC\docx_extracted.txt'
with open(out_path, 'w', encoding='utf-8') as f:
    f.write('\n'.join(output_lines))

print(f'Extracted {len(output_lines)} paragraphs')
print('DONE')
