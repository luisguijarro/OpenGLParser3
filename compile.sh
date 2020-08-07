#!/bin/sh

compi()
{
    echo "Compilando OpenGLParser 3"
    csc -nologo -recurse:./*.cs -out:./bin/Debug/oglp3.exe
    echo "Compilación Finalizada!"
}

case "$1" in
    -h)
    echo ""
    echo "\e[1m\e[38;2;255;128;128mOpenGL Parser 3 compiler script."
    echo "\e[38;2;128;128;255m==========================================="
    echo "\e[0muse -x argument for execute before compile."
    echo "You can use de exec arguments."
    echo "Example: \e[38;2;128;128;255m./compile.sh -x -d"
    echo "\e[0mThis example will compile de proyect and "
    echo "ejecute the result with -d argument."
    return 0;
    break
    ;;
    -x)
    compi
    cd ./bin/Debug
    mono oglp3.exe $2 $3 $4 $5 $6 $7 $8 $9
    cd ../..
    break
    ;;
    *)
    compi
    break;
    ;;
esac



#if [ "$1" = "x" ]
#then
#cd ./bin/Debug
#mono oglp3.exe $2 $3 $4 $5 $6 $7 $8 $9
#cd ../..
#fi