import {ParsedContent} from "@nuxt/content/dist/runtime/types";

export class ContentGroup {
    attributes: any;
    pages: Omit<ParsedContent,'body'>[];
    children: ContentGroup[];
    path: string;

    constructor(path: string) {
        this.path = path;
        this.attributes= {}
        this.pages = []
        this.children = []
    }

    addDirectory(dir: Omit<ParsedContent,'body'>) {
        const dirPath = dir._path?.replace('/_dir','')
        const cg = new ContentGroup(dirPath)
        cg.attributes.title = dir.title
        cg.attributes.icon = dir.icon
        cg.attributes.description = dir.description
        this.children.push(cg)
    }
    addPage(page: Omit<ParsedContent,'body'>) {
        const pagePath = page._path!
        const expectedDir = this.getDir(pagePath)
        if(this.path === expectedDir) {
            this.pages.push(page)
            return;
        }


        const dir = this.children.find(c => c.path === expectedDir)
        if(dir !== undefined)
            dir.pages.push(page)
    }

    private getDir(path:string) : string {
        return path.split('/').slice(0, -1).join('/')
    }

}

export function groupContent(items: Omit<ParsedContent,'body'>[]) : ContentGroup {
    const rootItem = items[0]
    const rootPath = rootItem._path?.replace('/_dir','')
    const result = new ContentGroup(rootPath)

    result.attributes.title = rootItem.title
    result.attributes.icon = rootItem.icon
    result.attributes.description = rootItem.description

    // pre-populate directory data
    items.slice(1).forEach(item => {
        if(item._path === undefined) return;

        if(item._path.endsWith('_dir')) {
            // we have a dir
            result.addDirectory(item)
        }

        if(item._id.includes('index')) {
            // this is the root page of the dir
            // result.addDirectory(item)
        }
    })

    items.slice(1)
        .filter(item => !item._path.endsWith('_dir'))
        .filter(item => !item._id.includes('index'))
        .forEach(item => {
            result.addPage(item)
        })

    return result;
}

function splitPath(path: string): string[] {
    return path.split('/')
}
