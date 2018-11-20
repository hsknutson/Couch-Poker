from sys import argv, exit, stderr
from os import chdir, listdir
from os.path import isfile, isdir

def help():
	print("Create production-ready html files by\n")
	print("inlining all local javascript or css files\n")
	print("and putting the resulting files into a build folder\n\n")
	print("usage: ./build.py <html file>\n")
	print("       --- OR ---\n")
	print("       ./build.py <folder name>\n")
	print("the latter will build each html file within the folder\n")

def build(file):
	if isdir(file):
		# ...
		chdir(file)
		for f in listdir("/"):
			if f.endswith(".html"):
				buildFile(f)
	else:
		buildFile(file)

def buildFile(file):
	pass

if __name__ == '__main__':
	if len(argv) == 1:
		help()
	elif len(argv) == 2:
		build(argv[1])