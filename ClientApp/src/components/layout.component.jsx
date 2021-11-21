import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './nav-menu.component';
import { Sidebar } from './sidebar.component';

import '../styles/Layout.css';

export const Layout = ({children}) => {
  const displayName = Layout.name;

  return (
    <div>
      <div className="layout">
        <Sidebar />
        <div style={{width: "100%"}}>
          <NavMenu />
          <Container>
            {children}
          </Container>        
        </div>
      </div>
    </div>
  );

}
// export class Layout extends Component {
//   static displayName = Layout.name;

//   render () {
//     return (
//       <div>
//         <div className="layout">
//           <Sidebar />
//           <div style={{width: "100%"}}>
//             <NavMenu />
//             <Container>
//               {this.props.children}
//             </Container>        
//           </div>
//         </div>
//       </div>
//     );
//   }
// }
