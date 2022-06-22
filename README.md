# Workshop

Diagrams - https://app.diagrams.net/?mode=github#Hitaitzruia4%2FWorkshop%2Fmain%2FWorkshop.drawio

Docs - https://docs.google.com/document/d/1uBLL_rCitwB7XsHgOPCzonL1Q7abzKXr6gH22F3790U/edit

# How to insert a config file to our system!
When starting the system, you can choose to pass the path to a config file as a command line argument. The config file must be of the format specified below.

# The config file format
You can type commands of the type:

admin\~[ADMIN_NAME]\~[ADMIN_PASS]\~[ADMIN_BIRTHDATE]

ss\~[STARTING_STATE_FILE_PATH]

Note, the admin command can be entered multiple times

# The Starting State file
The starting state file will be a file from which you can initialize the system. The file will contain multiple commands, each seperated by a newline character. A command will be of the format:

command(argument1,argument2,argument3,...)

Note, it is important that the arguments are seperated by commas, and without a space between the commas.

Each command will receive exactly what is expected of it in the Service in terms of parameters: If a command in the service receives int, string, int, it will expect int, string, int in the starting state file.

# Available commands:
enter-market

exit-market

register

login

logout

add-product

nominate-store-manager

nominate-store-owner

remove-store-owner-nomination

get-workers-information

close-store

open-store

create-new-store

review-product

search-product

get-all-stores

add-to-cart

view-cart

edit-cart

buy-cart

add-product-discount

add-category-discount

add-store-discount

remove-product-from-store

change-product-name

change-product-price

change-product-quantity

change-product-category

add-product-purchase-term

add-category-purchase-term

add-store-purchase-term

add-user-purchase-term

add-action-to-manager

take-notifications

get-members-online-stats

cancel-member
