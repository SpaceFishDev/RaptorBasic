section .bss
hello:
resb 3

[extern printf]
[extern scanf]
[extern exit]
section .data
int_string:
dw `%d`,0
read_in_st:
dw `%s`,0
a:
	dw `A`,0
b:
	dw `B`,0
c:
	dw 0
section .text
global main
main:
push eax
mov eax,  [a +     0]
mov [hello + 0],  eax
pop eax
push eax
mov eax,  [b +     0]
mov [hello + 1],  eax
pop eax
mov eax, [c]
add hello, eax
mov edx, dword c
mov hello, edx
sub hello, eax
push hello
call printf
push 0
call exit
