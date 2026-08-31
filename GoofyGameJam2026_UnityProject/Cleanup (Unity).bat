for /F "delims=" %%G in ('dir /a /b') do (
    if /I NOT "%%G"=="Assets" (
        if /I NOT "%%G"=="Packages" (
            if /I NOT "%%G"=="ProjectSettings" (
				if /I NOT "%%G"==".git" (
					if /I NOT "%%G"==".gitignore" (
						if /I NOT "%%G"=="Cleaner.bat" (
							if /I NOT "%%G"=="Builds" (
								if /I NOT "%%G"=="README.txt" (
									if /I NOT "%%G"=="README.md" (
										if /I NOT "%%G"=="Cleanup (Unity).bat" (
											IF EXIST "%%G\" (
												rmdir "%%G" /s /q
											) else (
												del "%%G" /q
											)
										)
									)
								)
							)
						)
					)
                )
            )
        )
    )
)