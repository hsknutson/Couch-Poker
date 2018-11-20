import re
from sys import argv, exit, stderr
from os import chdir, listdir, mkdir
from os.path import isfile, isdir

CSS_PATTERN = re.compile(r'<!-- css ([\w-]+.css) -->(.*)<!-- css \1 -->', re.DOTALL)
JS_PATTERN = re.compile(r'<!-- js ([\w-]+.js) -->(.*)<!-- js \1 -->', re.DOTALL)

def help():
	print("Create production-ready html files by")
	print("inlining all local javascript or css files")
	print("and putting the resulting files into a build folder\n")
	print("usage: ./build.py <html file>")
	print("       --- OR ---")
	print("       ./build.py <folder name>")
	print("the latter will build each html file within the folder")

def build(file):
	if isdir(file):
		# ...
		chdir(file)
		for f in listdir("./"):
			if f.endswith(".html"):
				buildFile(f)
	elif isfile(file):
		buildFile(file)
	else:
		print("Given file or directory doesn't exist.\n", file=stderr)
		exit(1)

def buildFile(file):
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
		
		if not isdir("build"):
			mkdir("build")

		with open("build/" + file, "w") as f2:
			f2.write(newData)



if __name__ == '__main__':
	if len(argv) == 1:
		help()
	elif len(argv) == 2:
		build(argv[1])