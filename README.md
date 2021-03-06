# NPIValidator

This is a simple NPI validation program, built using extensions.

## Background
The National Provider Identifier (NPI) is validated using the Luhn Formula for Modulus 10 “double-add-double” to calculate a Check Digit.
You can read [the official CMS requirements here.](https://www.cms.gov/Regulations-and-Guidance/Administrative-Simplification/NationalProvIdentStand/Downloads/NPIcheckdigit.pdf)
Following is the short version with example.

The Luhn check digit formula is calculated as follows: 
1. Double the value of alternate digits beginning with the rightmost digit.
1. Add the individual digits of the products resulting from step 1 to the unaffected digits from the original number.
1. Subtract the total obtained in step 2 from the next higher number ending in zero.  This is the check digit.
   If the total obtained in step 2 is a number ending in zero, the check digit is zero.

Example of Check Digit Calculation for NPI used without Prefix
Assume the 9-position identifier part of the NPI is 123456789.  
Using the Luhn formula on the identifier portion, the check digit is calculated as follows:
NPI without check digit:
`1     2     3     4     5     6     7     8     9 `

Step 1: Double the value of alternate digits, beginning with the rightmost digit.
`2            6          10          14          18 `

Step 2:  Add constant 24, to account for the 80840 prefix that would be present on a card issuer identifier, 
plus the individual digits of products of doubling, plus unaffected digits.
`24 + 2 + 2 + 6 + 4 + 1 + 0 + 6 + 1 + 4 + 8 + 1 + 8 = 67 `

Step 3:  Subtract from next higher number ending in zero. 
`70 – 67 = 3 `
Check digit = `3 `
NPI with check digit = `1234567893 `

## References
Some of the great resources that helped me to get this working:
* [how to find the next multiple of 10, of a number](https://stackoverflow.com/questions/2403631/how-do-i-find-the-next-multiple-of-10-of-any-integer#2403643)
* [how to select every other element from an array](https://stackoverflow.com/questions/13082913/select-every-second-element-from-array-using-lambda#13082944)
