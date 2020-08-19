#!/bin/sh

widthterminal=$(($(stty size | cut '-d ' -f2)))
barrachar='='
barra=''
cont=0
while [ $cont -lt $widthterminal ]
do
    barra=$barra$barrachar
    cont=$(($cont+1))
done

makedir()
{
    if [ ! -d $1 ]
    then
        mkdir $1
    fi
}

showhead()
{
    echo ''
    echo "\e[1m\e[38;2;255;128;128mOpenGL Parser 3 compiler script."
    echo "\e[38;2;128;128;255m"$barra
    if [ "$1" = "-d" ]
    then
        echo "\e[0mCompiling in Debug Mode."
    else
        echo "\e[0mCompiling in Normal Mode."
    fi

    echo "\e[1m\e[38;2;128;128;255m"$barra"\e[0m"
}






compi()
{
    makedir "./bin"
    makedir "./bin/Debug"
    csc -nologo -recurse:./*.cs -out:./bin/Debug/oglp3.exe
    echo "Compilación Finalizada!"
    echo ""
}

compid()
{
    makedir "./bin"
    makedir "./bin/Debug"
    csc -nologo -debug:full -recurse:./*.cs -out:./bin/Debug/oglp3.exe
    echo "Compilación Finalizada!"
    echo ""
}

execute()
{
    cd ./bin/Debug
    echo "Runing output..."
    echo ""
    mono oglp3.exe $1 $2 $3 $4 $5 $6 $7 $8 $9
    cd ../..
}

case "$1" in
    -h)
    echo ""
    echo "\e[1m\e[38;2;255;128;128mOpenGL Parser 3 compiler script."
    echo "\e[38;2;128;128;255m"$barra
    echo "\e[0muse -x argument for execute before compile."
    echo "       You can use de exec arguments."
    echo "       Example: \e[38;2;128;128;255m./compile.sh -x -d"
    echo "       \e[0mThis example will compile de proyect and "
    echo "       ejecute the result with -d argument."

    echo "\e[0muse -d argument for compile in debug mode."
    echo "       -d argument always must be before -x argument."
    return 0;
    break
    ;;
    -x)
    showhead 
    compi
    execute $2 $3 $4 $5 $6 $7 $8 $9
    break
    ;;
    -d)
    showhead -d
    compid
    if [ "$2" = "-x" ]
    then
        execute $3 $4 $5 $6 $7 $8 $9
    fi
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