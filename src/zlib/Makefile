CC=gcc
CFLAGS=-I. -O2 -Wall
DEPS = crc32.h deflate.h gzguts.h inffast.h inffixed.h inflate.h \
	   inftrees.h trees.h zconf.h zlib.h zutil.h

OBJ = adler32.o compress.o crc32.o deflate.o gzclose.o gzlib.o gzread.o gzwrite.o \
	  infback.o inffast.o inflate.o inftrees.o trees.o uncompr.o zutil.o

%.o: %.c $(DEPS)
	$(CC) $(CFLAGS) -c $< -o $@ 

default: libzlib.a

clean:
	rm -f $(OBJ)
	rm -f libzlib.a

libzlib.a: $(OBJ)
	ar rcs $@ $^
	ranlib $@
