
### Initial setup

Install the generator tool and dependencies.
```
npm install
```

### Ongoing testing

Run the generator against sb-end-to-end.yml
```
npm run generate-template
```
The resulting ARM template can be found at output/deploy.json

### Clean output (usually not needed)
Removes the output directory.
```
npm run clean
```
