import os, pathlib

def find_duplicate_names_in_folders(directory):
    files_by_path = set()

    # Walk through the directory
    for path in pathlib.Path(directory).rglob("*"):
        if path.is_file():
            files_by_path.add(path)
    
    orphaned = set()
    for path in files_by_path:
        if path.suffix == ".uid":
            check_path = path.parent / path.stem
            if check_path not in files_by_path:
                orphaned.add(path)
    
    return orphaned

# Replace 'your_directory_path' with the path you want to search
os.curdir = os.path.dirname(__file__)
directory_path = '../'
duplicates = find_duplicate_names_in_folders(directory_path)

if duplicates:
    print(f"Found {len(duplicates)} orphaned:")
    for file in duplicates:
        print(f"Duplicate name found: {file}")
        #for file in files:  print(f"  - {file}")
    while True:
        r = input("Delete y/n?")
        if r == 'y':
            for file in duplicates:
                os.remove(file)
            print("Orphaned uids deleted.")
            break
        elif r == 'n':
            print("Not deleted.")
            break
        else:
            print("Invalid answer.")
else:
    print("No duplicates found.")
input("press Enter to exit...")
