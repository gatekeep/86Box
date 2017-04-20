CC=gcc
CFLAGS=-I. -O2 -Wall
DEPS = SDL_net.h SDLnetsys.h

OBJ = SDLnet.c SDLnetselect.c SDLnetTCP.c SDLnetUDP.c

%.o: %.c $(DEPS)
	$(CC) $(CFLAGS) -c $< -o $@ 

default: libsdlnet.a

clean:
	rm -f $(OBJ)
	rm -f libsdlnet.a

libsdlnet.a: $(OBJ)
	ar rcs $@ $^
	ranlib $@
