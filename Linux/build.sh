gcc -Wall -Wno-unknown-pragmas \
    -O2 -fPIC -Wl,--gc-sections \
    -I../Source \
    -D RTMIDI_EXPORT -D __LINUX_ALSA__ \
    ../Source/RtMidi.cpp \
    ../Source/rtmidi_c.cpp \
    -lstdc++ \
    -shared -o libRtMidi.so
