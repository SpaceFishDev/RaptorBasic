section .bss

[extern printf]
[extern scanf]
[extern exit]
section .data
int_string:
dw `%d`,0
read_in_st:
dw `%s`,0
a:
	dd 32
b:
	dd 32
section .text
global main
main:
mov eax, dword [a]
mov edx, dword 12
sub eax, edx
mov [a], dword eax
push dword [a]
push dword int_string
call printf
push 0
call exit
