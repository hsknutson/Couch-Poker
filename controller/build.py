import re
from sys import argv, exit, stderr
from os import chdir, listdir, mkdir
from os.path import isfile, isdir

"""
The build script for the production controller html file

Python 3 is needed to run

Usage: python3 build.py controller.html

Will output the html file to build/controller.html
"""

CSS_PATTERN = re.compile(r'<!-- css (.*.css) -->(.*)<!-- css \1 -->', re.DOTALL)
JS_PATTERN = re.compile(r'<!-- js ([\w-]+.js) -->(.*)<!-- js \1 -->', re.DOTALL)

def help():
	print("\t-------------------- Build.py Help --------------------\n")
	print("\tCreate production-ready html files by")
	print("\tinlining all local javascript or css files")
	print("\tand putting the resulting files into a build folder\n")
	print("\tusage: python3 build.py <html file> <optional output folder>")
	print("\t       --- OR ---")
	print("\t       python3 build.py <folder name> <optional output folder>\n")
	print("\tthe latter will build each html file within the folder\n")
	print("\tto ouput in the parent folder, do: ")
	print("\t\tpython3 build.py <html file> ../")

def build(file, outFolder="build"):
	if isdir(file):
		chdir(file)
		for f in listdir("./"):
			if f.endswith(".html"):
				buildFile(f, outFolder)
	elif isfile(file):
		buildFile(file, outFolder)
	else:
		print("Given file or directory doesn't exist.\n", file=stderr)
		exit(1)

def buildFile(file, outFolder):
	with open(file, "r") as f:
		data = f.read()
		newData = ""
		currentIndex = 0

		css = CSS_PATTERN.finditer(data)
		for i in css:
			newData += data[currentIndex:i.start()-1]
			with open(i.group(1), "r") as f1:
				newData += "<style>\n" + f1.read() + "</style>\n"

			currentIndex = i.end()+1

		js = JS_PATTERN.finditer(data)
		for i in js:
			newData += data[currentIndex:i.start()-1]
			with open(i.group(1), "r") as f1:
				newData += "<script>\n" + f1.read() + "</script>\n"

			currentIndex = i.end()+1

		newData += data[currentIndex:]
		
		if not isdir(outFolder):
			mkdir(outFolder)

		with open(outFolder + "/" + file, "w") as f2:
			f2.write(newData)



if __name__ == '__main__':
	if len(argv) == 1:
		help()
	elif len(argv) == 2:
		build(argv[1])
	elif len(argv) == 3:
		build(argv[1], argv[2])