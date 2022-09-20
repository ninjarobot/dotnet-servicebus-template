import { File, Text, Indent, IndentationTypes } from '@asyncapi/generator-react-sdk';
import { Template } from '../components/Template'
import { Namespace } from '../components/Namespace'

export default function ({ asyncapi }) {
    return <File name={'deploy.json'}>
        <Template asyncapi={asyncapi}>
            <Namespace server={asyncapi.servers.name}/>
        </Template>
    </File>;
}