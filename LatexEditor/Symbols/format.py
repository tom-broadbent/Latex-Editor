import argparse
import re

# A script for formatting text copied from
# https://www.overleaf.com/learn/latex/List_of_Greek_letters_and_math_symbols

parser = argparse.ArgumentParser()
parser.add_argument("filename")
parser.add_argument("-v", "--verbose", action="store_true")
args = parser.parse_args()

replacements = {
    "\\displaystyle": "",
    "{": "",
    "}": "",
    " ": "\n",
    "\\;": ""
}

with open(args.filename, "r") as i:
    lines = i.read()
    
if args.verbose:
    print("==========================")
    print(lines)

lines = lines.replace("\t", "\n")
lines = lines.split("\n")

if args.verbose:
    print("==========================")
    print(lines)

newlines = []
for line in lines:
    newline = re.sub(r'{(.*)}', '', line)
    if len(newline) > 0:
        newline = re.split(r'(?<=[a-z])(?=[A-Z]|\\)', newline)
        newlines += newline
    
if args.verbose:
    print("==========================")
    print(newlines)
    
with open(args.filename, 'w') as o:
    o.write("\n".join(newlines))
    